namespace OrderService.Services
{
    public interface IMenuServiceClient
    {
        Task<bool> DeductStockAsync(int menuItemId, int quantity, string token);
    }
}