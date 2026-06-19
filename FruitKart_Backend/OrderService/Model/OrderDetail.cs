using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OrderService.Model
{
    [Table("tbl_OrderDetail")]
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }
        [Required]
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        [JsonIgnore]  // breaks the cycle
        public OrderHeader OrderHeader { get; set; } = null!; [Required]
        public int MenuItemId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string ItemName { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        public int? Rating { get; set; } = null;
    }
}
