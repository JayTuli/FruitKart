using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountService.Models
{
    [Table("tbl_User")]

    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MaxLength(256)]
        public string Password { get; set; }

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [MaxLength(256)]
        public string Address { get; set; }

        public DateTime DateRegistered { get; set; } = DateTime.UtcNow;
    }
}
