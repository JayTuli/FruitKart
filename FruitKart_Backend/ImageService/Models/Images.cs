using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageService.Models
{
    [Table("tbl_Images")]
    public class Images
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ImageName { get; set; } = string.Empty;

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public int FruitId { get; set; }
        [Required]
        public string PublicId { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}