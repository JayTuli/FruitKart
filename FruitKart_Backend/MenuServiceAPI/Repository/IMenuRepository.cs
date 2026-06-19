using MenuServiceAPI.Models;
using MenuServiceAPI.Models.DTO;

namespace MenuServiceAPI.Repository
{
    public interface IMenuRepository
    {
        Task<List<MenuItemDTO>> GetAllAsync();
        Task<MenuItemDTO?> GetByIdAsync(int id);
        Task<MenuItemDTO> CreateAsync(MenuItemCreateDTO createDTO, string imageUrl);
        Task<MenuItemDTO?> UpdateAsync(int id, MenuItemUpdateDTO updateDTO, string? imageUrl);
        Task<(DeductStockResult Result, int StockCount)> DeductStockAsync(int id, int quantity);
        Task<bool> DeleteAsync(int id);
    }
}