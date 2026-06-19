using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AccountService.Models
{
    public class ApplicationUser : IdentityUser
    {
        //IdentityUser already has Email, PhoneNumber, PasswordHash, UserName — no need to redefine them
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
