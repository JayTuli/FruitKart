using System.ComponentModel.DataAnnotations;

namespace ChatBotService.Models.DTO
{
    public class ChatRequestDTO
    {
        [Required]
        public string Message { get; set; } = string.Empty;
    }
}
