using System.ComponentModel.DataAnnotations;

namespace AccountService.Models.DTO
{
    public class UpdateUserDTO
    {
        public string Address { get; set; }
        [MaxLength(15)]

        [Phone]
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must contain digits only")]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Name { get; set; }
    }
}
