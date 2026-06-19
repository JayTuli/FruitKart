using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OrderService.Services
{
    public class MenuServiceClient : IMenuServiceClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<MenuServiceClient> _logger;

        public MenuServiceClient(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<MenuServiceClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
        }

        public async Task<bool> DeductStockAsync(int menuItemId, int quantity, string token)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("MenuService");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var body = new StringContent(
                    JsonSerializer.Serialize(quantity),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PatchAsync(
                    $"/api/menuitem/{menuItemId}/stock", body);

                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    _logger.LogWarning(
                        "Insufficient stock for MenuItemId {Id}", menuItemId);
                    return false;
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to deduct stock for MenuItemId {Id}", menuItemId);
                return false;
            }
        }
    }
}