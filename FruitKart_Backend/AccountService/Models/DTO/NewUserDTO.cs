using System.ComponentModel.DataAnnotations;

namespace AccountService.Models.DTO
{
    public class NewUserDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must contain digits only")]
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = "Not Provided";
    }
}
