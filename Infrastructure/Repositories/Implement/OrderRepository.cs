using Application.Commons;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Response.OrderResponse;
using static Application.ViewModel.Response.ProductResponse;

namespace Infrastructure.Repositories.Implement
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) => _context = context;

        public async Task<Pagination<OrderResultDTO>> GetAllOrdersAsync(int pageIndex, int pageSize)
        {
            var totalItemCount = await _context.Order.CountAsync();

            var orders = await _context.Order
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.CreatedAt) // Sắp xếp theo ngày tạo mới nhất
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderResultDTO
                {
                    TotalPrice = o.TotalPrice,
                    Email = o.Customer.AccountProfile.Email,
                    CreatedAt = o.CreatedAt.HasValue ? o.CreatedAt.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null, // Fix lỗi
                    OrderItems = o.OrderDetails.Select(od => new ViewProductDTO
                    {
                        ProductName = od.Product.ProductName,
                        Price = od.UnitPrice,
                        StockQuantity = od.Quantity
                    }).ToList()
                })
                .ToListAsync();

            return new Pagination<OrderResultDTO>
            {
                TotalItemCount = totalItemCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = orders
            };
        }


    }
}
