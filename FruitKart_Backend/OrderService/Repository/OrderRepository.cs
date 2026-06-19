using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Model;
using OrderService.Model.DTO;
using OrderService.Services;
using System.ComponentModel.Design;

namespace OrderService.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _db;
        private readonly IMenuServiceClient _menuServiceClient;

        public OrderRepository(OrderDbContext db, IMenuServiceClient menuServiceClient)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _menuServiceClient = menuServiceClient  
           ?? throw new ArgumentNullException(nameof(menuServiceClient));
        }

        public async Task<IEnumerable<OrderHeader>> GetAllOrdersAsync(string userId = "")
        {
            var query = _db.OrderHeaders
                           .Include(u => u.OrderDetails)
                           .OrderByDescending(u => u.OrderHeaderId)
                           .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(u => u.ApplicationUserId == userId);

            return await query.ToListAsync();
        }

        public async Task<OrderHeader?> GetOrderByIdAsync(int orderId)
        {
            return await _db.OrderHeaders
                            .Include(u => u.OrderDetails)
                            .FirstOrDefaultAsync(u => u.OrderHeaderId == orderId);
        }

        public async Task<OrderHeader> CreateOrderAsync(OrderHeaderCreateDTO dto, string userId, string token)
        {
            foreach (var detail in dto.OrderDetailDTO)
            {
                var stockOk = await _menuServiceClient
                    .DeductStockAsync(detail.MenuItemId, detail.Quantity, token);

                if (!stockOk)
                    throw new InvalidOperationException(
                        $"Insufficient stock for item '{detail.ItemName}'.");
            }
            var orderHeader = new OrderHeader
            {
                PickUpName = dto.PickUpName,
                PickUpPhoneNumber = dto.PickUpPhoneNumber,
                PickUpEmail = dto.PickUpEmail,
                OrderDate = DateTime.UtcNow,
                OrderTotal = dto.OrderDetailDTO.Sum(d => d.Price * d.Quantity), // server-side calc
                Status = OrderStatus.Confirmed,
                TotalItem = dto.TotalItem,
                ApplicationUserId = userId  // from JWT
            };

            _db.OrderHeaders.Add(orderHeader);
            await _db.SaveChangesAsync();

            foreach (var detailDTO in dto.OrderDetailDTO)
            {
                _db.OrderDetails.Add(new OrderDetail
                {
                    OrderHeaderId = orderHeader.OrderHeaderId,
                    MenuItemId = detailDTO.MenuItemId,
                    Quantity = detailDTO.Quantity,
                    ItemName = detailDTO.ItemName,
                    Price = detailDTO.Price
                });
            }

            await _db.SaveChangesAsync();
            return orderHeader;
        }

        public async Task<OrderHeader?> UpdateOrderAsync(int orderId, OrderHeaderUpdateDTO dto)
        {
            var order = await _db.OrderHeaders
                .FirstOrDefaultAsync(u => u.OrderHeaderId == orderId);
            if (order is null) return null;

            if (!string.IsNullOrEmpty(dto.PickUpName))
                order.PickUpName = dto.PickUpName;

            if (!string.IsNullOrEmpty(dto.PickUpPhoneNumber))
                order.PickUpPhoneNumber = dto.PickUpPhoneNumber;

            if (!string.IsNullOrEmpty(dto.PickUpEmail))
                order.PickUpEmail = dto.PickUpEmail;

            if (!string.IsNullOrEmpty(dto.Status))
            {
                var current = order.Status;
                var next = dto.Status;

                // Full status transition flow

                if (current.Equals(OrderStatus.Confirmed, StringComparison.InvariantCultureIgnoreCase)
                    && next.Equals(OrderStatus.ReadyForPickup, StringComparison.InvariantCultureIgnoreCase))
                    order.Status = OrderStatus.ReadyForPickup;

                else if (current.Equals(OrderStatus.ReadyForPickup, StringComparison.InvariantCultureIgnoreCase)
                    && next.Equals(OrderStatus.Completed, StringComparison.InvariantCultureIgnoreCase))
                    order.Status = OrderStatus.Completed;

                else if (next.Equals(OrderStatus.Cancelled, StringComparison.InvariantCultureIgnoreCase))
                    order.Status = OrderStatus.Cancelled;  //  cancel from any state
            }

            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<OrderDetail?> UpdateOrderDetailRatingAsync(int orderDetailId, OrderDetailUpdateDTO dto)
        {
            var detail = await _db.OrderDetails
                .FirstOrDefaultAsync(u => u.OrderDetailId == orderDetailId);
            if (detail is null) return null;

            detail.Rating = dto.Rating;
            await _db.SaveChangesAsync();
            return detail;
        }
    }
}