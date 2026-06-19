namespace ChatBotService.Services
{
    public interface IChatBotService
    {
        // Added userId + token so chatbot can fetch user's own orders securely
        Task<string> GetChatResponseAsync(string userMessage, string userId, string token);
    }
}