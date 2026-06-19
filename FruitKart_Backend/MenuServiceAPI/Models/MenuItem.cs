using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenuServiceAPI.Models
{
    [Table("tbl_MenuItem")]
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public string Category { get; set; } = string.Empty;
        public string? SpecialTag { get; set; }
        [Range(1, 1000)]
        public double Price { get; set; }
        [Required]
        [MaxLength(500)]
        public string Image { get; set; } = string.Empty;
        [NotMapped]
        public double Rating { get; set; }

        [Range(1,10000)]
        public int StockCount {  get; set; } =0;

    }
}
