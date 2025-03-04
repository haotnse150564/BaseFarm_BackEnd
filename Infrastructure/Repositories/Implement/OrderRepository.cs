using Application.Commons;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
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
                    Email = o.Customer.Email,
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

        public async Task<Pagination<OrderResultDTO>> GetOrdersByCustomerIdAsync(long customerId, int pageIndex, int pageSize)
        {
            var totalItemCount = await _context.Order
                .Where(o => o.CustomerId == customerId)
                .CountAsync();

            var orders = await _context.Order
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderResultDTO
                {
                    TotalPrice = o.TotalPrice,
                    Email = o.Customer.Email,
                    CreatedAt = o.CreatedAt.HasValue ? o.CreatedAt.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
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

        public async Task<OrderResultDTO> GetOrderByIdAsync(long orderId)
        {
            var order = await _context.Order
                .Where(o => o.OrderId == orderId)
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Select(o => new OrderResultDTO
                {
                    TotalPrice = o.TotalPrice,
                    Email = o.Customer.Email,
                    CreatedAt = o.CreatedAt.HasValue ? o.CreatedAt.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    OrderItems = o.OrderDetails.Select(od => new ViewProductDTO
                    {
                        ProductName = od.Product.ProductName,
                        Price = od.UnitPrice,
                        StockQuantity = od.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return order;
        }

        public async Task<Order?> GetOrderById(long orderId)
        {
            return await _context.Order
                .Where(o => o.OrderId == orderId)
                .OrderByDescending(o => o.CreatedAt) // Sắp xếp theo ngày mới nhất
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync();
        }

    }
}
