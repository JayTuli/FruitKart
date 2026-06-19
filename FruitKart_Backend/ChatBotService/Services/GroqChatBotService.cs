using ChatBotService.Services;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ChatBotService.Services
{
    public class GroqChatBotService : IChatBotService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<GroqChatBotService> _logger;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _systemPrompt;
        private readonly string _menuBaseUrl;
        private readonly string _orderBaseUrl;

        private const string GroqApiUrl =
            "https://api.groq.com/openai/v1/chat/completions";

        private static readonly string[] OrderKeywords =
        {
            "order", "status", "track", "delivery", "placed",
            "when", "ready", "pickup", "pending", "confirmed",
            "my order", "order id", "cancelled"
        };

        public GroqChatBotService(
            IConfiguration config,
            ILogger<GroqChatBotService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpClientFactory = httpClientFactory
                ?? throw new ArgumentNullException(nameof(httpClientFactory));

            _apiKey = config["Groq:ApiKey"]
                ?? throw new InvalidOperationException("Groq:ApiKey not configured");

            _model = config["Groq:Model"] ?? "llama-3.3-70b-versatile";
            _systemPrompt = config["Groq:SystemPrompt"] ?? "You are a helpful AI assistant.";
            _menuBaseUrl = config["MenuService:BaseUrl"] ?? "http://localhost:5142";
            _orderBaseUrl = config["OrderService:BaseUrl"] ?? "http://localhost:5268";
        }

        // ─── Main method ─────────────────────────────────────────────────────
        public async Task<string> GetChatResponseAsync(
            string userMessage,
            string userId,
            string token)
        {
            try
            {
                var menuContext = await GetMenuContextAsync();

                var orderContext = string.Empty;
                if (IsOrderRelated(userMessage))
                    orderContext = await GetOrderContextAsync(userId, token);

                var sb = new StringBuilder();
                sb.AppendLine(_systemPrompt);
                sb.AppendLine();
                sb.AppendLine("--- CURRENT MENU FROM DATABASE ---");
                sb.AppendLine(menuContext);
                sb.AppendLine("--- END OF MENU ---");

                if (!string.IsNullOrEmpty(orderContext))
                {
                    sb.AppendLine();
                    sb.AppendLine("--- THIS USER'S ORDERS FROM DATABASE ---");
                    sb.AppendLine(orderContext);
                    sb.AppendLine("--- END OF ORDERS ---");
                    sb.AppendLine();
                    sb.AppendLine("IMPORTANT: Only share order details that belong to this user.");
                    sb.AppendLine("Never reveal other users' order information.");
                }

                var requestBody = new
                {
                    model = _model,
                    messages = new[]
                    {
                        new { role = "system", content = sb.ToString() },
                        new { role = "user",   content = userMessage   }
                    },
                    temperature = 0.7,
                    max_tokens = 512
                };

                // Fresh client per request — no shared state issues
                var groqClient = _httpClientFactory.CreateClient();
                groqClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _apiKey);

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await groqClient.PostAsync(GroqApiUrl, content);
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(body);

                var reply = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return string.IsNullOrWhiteSpace(reply)
                    ? "Sorry, I couldn't generate a response."
                    : reply;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetChatResponseAsync");
                throw;
            }
        }

        private static bool IsOrderRelated(string message)
        {
            var lower = message.ToLowerInvariant();
            return OrderKeywords.Any(k => lower.Contains(k));
        }

        // ─── Fetches orders — fresh client + fresh headers every time ────────
        private async Task<string> GetOrderContextAsync(string userId, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                    return "No order information available. Please log in.";

                // Create a FRESH client for this request — avoids mutation issues
                var orderClient = _httpClientFactory.CreateClient();
                orderClient.BaseAddress = new Uri(_orderBaseUrl);
                orderClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                _logger.LogInformation(
                    "Calling Order Service at {Url}/api/orderheader for user {UserId}",
                    _orderBaseUrl, userId);

                var response = await orderClient.GetAsync("/api/orderheader");

                _logger.LogInformation(
                    "Order Service responded with {StatusCode}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Order Service returned {Status}", response.StatusCode);
                    return "Could not fetch order information at this time.";
                }

                var json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Order Service raw response: {Json}", json);

                using var doc = JsonDocument.Parse(json);

                // Handle all 4 possible response shapes
                var orderList = new List<JsonElement>();

                if (doc.RootElement.TryGetProperty("result", out var resultProp))
                {
                    if (resultProp.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var o in resultProp.EnumerateArray())
                            orderList.Add(o);
                    }
                    else if (resultProp.ValueKind == JsonValueKind.Object)
                    {
                        // Single order object inside result — YOUR CASE
                        orderList.Add(resultProp);
                    }
                }
                else if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var o in doc.RootElement.EnumerateArray())
                        orderList.Add(o);
                }
                else if (doc.RootElement.ValueKind == JsonValueKind.Object
                         && doc.RootElement.TryGetProperty("orderHeaderId", out _))
                {
                    orderList.Add(doc.RootElement);
                }

                _logger.LogInformation("Parsed {Count} orders for user {UserId}",
                    orderList.Count, userId);

                if (orderList.Count == 0)
                    return "This user has no orders yet.";

                var sb = new StringBuilder();
                foreach (var order in orderList)
                {
                    var id = order.TryGetProperty("orderHeaderId", out var i)
                        ? i.GetInt32().ToString() : "?";
                    var status = order.TryGetProperty("status", out var s)
                        ? s.GetString() : "Unknown";
                    var total = order.TryGetProperty("orderTotal", out var t)
                        ? t.GetDecimal().ToString("F2") : "0.00";
                    var items = order.TryGetProperty("totalItem", out var c)
                        ? c.GetInt32().ToString() : "0";
                    var date = order.TryGetProperty("orderDate", out var d)
                        ? d.GetString() : "";

                    var dateStr = DateTime.TryParse(date, out var dt)
                        ? dt.ToString("dd MMM yyyy, hh:mm tt") : date;

                    sb.AppendLine(
                        $"- Order #{id} | Status: {status} | " +
                        $"Total: Rs.{total} | Items: {items} | Date: {dateStr}");
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not fetch orders for user {UserId}", userId);
                return "Could not fetch order information at this time.";
            }
        }

        // ─── Fetches fruit menu — fresh client ───────────────────────────────
        private async Task<string> GetMenuContextAsync()
        {
            try
            {
                // Fresh client — no shared state
                var menuClient = _httpClientFactory.CreateClient();
                menuClient.BaseAddress = new Uri(_menuBaseUrl);

                var response = await menuClient.GetAsync("/api/menuItem");

                if (!response.IsSuccessStatusCode)
                    return "Menu data currently unavailable.";

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                JsonElement items;
                if (doc.RootElement.TryGetProperty("result", out var resultProp)
                    && resultProp.ValueKind == JsonValueKind.Array)
                    items = resultProp;
                else if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    items = doc.RootElement;
                else
                    return "Menu data unavailable.";

                var sb = new StringBuilder();
                foreach (var item in items.EnumerateArray())
                {
                    var name = item.TryGetProperty("name", out var n) ? n.GetString() : "Unknown";
                    var price = item.TryGetProperty("price", out var p) ? p.GetDecimal() : 0;
                    var cat = item.TryGetProperty("category", out var c) ? c.GetString() : "";
                    var tag = item.TryGetProperty("specialTag", out var s) ? s.GetString() : "";

                    sb.AppendLine(
                        $"- {name} | Rs.{price}" +
                        (string.IsNullOrEmpty(cat) ? "" : $" | {cat}") +
                        (string.IsNullOrEmpty(tag) ? "" : $" | {tag}"));
                }

                return sb.Length > 0 ? sb.ToString() : "No menu items available.";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not fetch menu");
                return "Menu data currently unavailable.";
            }
        }
    }
}