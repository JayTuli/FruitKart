using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Model
{
    [Table("tbl_OrderHeader")]
    public class OrderHeader
    {
        [Key]
        public int OrderHeaderId { get; set; }
        [Required]
        public string PickUpName { get; set; } = string.Empty;
        [Required]
        public string PickUpPhoneNumber { get; set; } = string.Empty;
        [Required]
        public string PickUpEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;
        
        public decimal OrderTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalItem { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}
