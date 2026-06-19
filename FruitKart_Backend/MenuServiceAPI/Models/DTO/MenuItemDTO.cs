namespace MenuServiceAPI.Models.DTO
{
    public class MenuItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? SpecialTag { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; } = string.Empty;  
        public int StockCount { get; set; }
        public double Rating { get; set; }
    }
}
