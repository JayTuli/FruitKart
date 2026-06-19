using AutoMapper;
using MenuServiceAPI.Data;
using MenuServiceAPI.Models;
using MenuServiceAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace MenuServiceAPI.Repository
{
    public class MenuRepository : IMenuRepository
    {
        private readonly MenuDbContext _db;
        private readonly IMapper _mapper;

        public MenuRepository(MenuDbContext db, IMapper mapper)  
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<MenuItemDTO>> GetAllAsync()
        {
            var items = await _db.MenuItems.AsNoTracking().ToListAsync();
            return _mapper.Map<List<MenuItemDTO>>(items);
        }

        public async Task<MenuItemDTO?> GetByIdAsync(int id)
        {
            var item = await _db.MenuItems.AsNoTracking()
                                          .FirstOrDefaultAsync(m => m.Id == id);
            return item is null ? null : _mapper.Map<MenuItemDTO>(item);
        }

        public async Task<MenuItemDTO> CreateAsync(MenuItemCreateDTO createDTO, string imageUrl)  
        {
            var menuItem = _mapper.Map<MenuItem>(createDTO);
            menuItem.Image = imageUrl;
            await _db.MenuItems.AddAsync(menuItem);
            await _db.SaveChangesAsync();
            return _mapper.Map<MenuItemDTO>(menuItem);
        }
        public async Task<(DeductStockResult Result,int StockCount)> DeductStockAsync(int id, int quantity)
        {
            var item = await _db.MenuItems.FirstOrDefaultAsync(m => m.Id == id);
            if (item is null) return (DeductStockResult.NotFound,0);

            if (item.StockCount < quantity)
                return (DeductStockResult.InsufficientStock, item.StockCount);

            item.StockCount -= quantity;
            await _db.SaveChangesAsync();
            return (DeductStockResult.Success, item.StockCount);
        }
        public async Task<MenuItemDTO?> UpdateAsync(int id, MenuItemUpdateDTO updateDTO, string? imageUrl) 
        {
            var existing = await _db.MenuItems.FirstOrDefaultAsync(m => m.Id == id);
            if (existing is null) return null;

            existing.Name = updateDTO.Name;
            existing.Description = updateDTO.Description;
            existing.Category = updateDTO.Category;
            existing.SpecialTag = updateDTO.SpecialTag;
            existing.Price = updateDTO.Price;
            existing.StockCount = updateDTO.StockCount;

            // Image updated separately via ImageService    
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                existing.Image = imageUrl;
            }

            await _db.SaveChangesAsync();
            return _mapper.Map<MenuItemDTO>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _db.MenuItems.FirstOrDefaultAsync(m => m.Id == id);
            if (item is null) return false;

            _db.MenuItems.Remove(item);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}