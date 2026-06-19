using ChatBotService.Models;
using ChatBotService.Models.DTO;
using ChatBotService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChatBotService.Controllers
{
    [Route("api/chatbot")]
    [ApiController]
    [Authorize]
    public class ChatBotController : ControllerBase
    {
        private readonly IChatBotService _chatBotService;
        private readonly ILogger<ChatBotController> _logger;

        public ChatBotController(
            IChatBotService chatBotService,
            ILogger<ChatBotController> logger)
        {
            _chatBotService = chatBotService
                ?? throw new ArgumentNullException(nameof(chatBotService));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequestDTO request)
        {
            var response = new ApiResponse();

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Message))
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Message cannot be empty.");
                return BadRequest(response);
            }

            try
            {
                // Extract userId from JWT claim
                var userId = User.FindFirst("UserId")?.Value ?? "";

                // Extract raw Authorization header — preserve exactly as received
                var authHeader = Request.Headers["Authorization"].ToString();

                _logger.LogInformation(
                    "SendMessage — userId={UserId} | authHeader present={Present} | length={Len}",
                    userId,
                    !string.IsNullOrEmpty(authHeader),
                    authHeader.Length);

                // Strip "Bearer " prefix (case-insensitive, trimmed)
                var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? authHeader.Substring(7).Trim()
                    : authHeader.Trim();

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Token is empty after extraction — cannot forward to Order Service");
                }
                else
                {
                    _logger.LogInformation("Token extracted successfully, length={Len}", token.Length);
                }

                var chatResponse = await _chatBotService
                    .GetChatResponseAsync(request.Message, userId, token);

                response.Result = new ChatResponseDTO { Response = chatResponse };
                response.StatusCode = HttpStatusCode.OK;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendMessage");
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Failed to get chatbot response. Please try again.");
                return StatusCode(500, response);
            }
        }

        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult Health()
        {
            return Ok(new ApiResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = new { message = "ChatBotService is running" }
            });
        }
    }
}