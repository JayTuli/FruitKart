using System.ComponentModel.DataAnnotations;

namespace ImageService.Models.DTO
{
    public class NewImagesDTO
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}