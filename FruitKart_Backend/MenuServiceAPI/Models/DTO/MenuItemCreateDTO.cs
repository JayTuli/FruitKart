using System.ComponentModel.DataAnnotations;

namespace MenuServiceAPI.Models.DTO
{
    public class MenuItemCreateDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public string Category { get; set; } = string.Empty;
        public string? SpecialTag { get; set; }
        [Range(1, 1000)]
        public decimal Price { get; set; }
        public IFormFile File { get; set; } = null!;

        [Range(0, 10000)]
        public int StockCount { get; set; } = 0;

    }
}
