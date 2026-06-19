namespace AccountService.Models.DTO
{
    public class LoginResponseDTO
    {
        public int UserId { get; set; }
        //public string? Token { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }   // for errors
        public string? Token { get; set; }     // only populated on success
        public string? Role { get; set; }
    }
}
