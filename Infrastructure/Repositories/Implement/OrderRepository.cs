using Application.Commons;
using Domain.Enum;
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

        public async Task<Pagination<OrderResultDTO>> GetAllOrdersAsync(int pageIndex, int pageSize, Status? status)
        {
            var query = _context.Order.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status);
            }

            var totalItemCount = await query.CountAsync();

            var orders = await query
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
                    ShippingAddress = o.ShippingAddress,
                    Status = o.Status,
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


        public async Task<Pagination<OrderResultDTO>> GetOrdersByCustomerIdAsync(long customerId, int pageIndex, int pageSize, Status? status)
        {
            var query = _context.Order
                .Where(o => o.CustomerId == customerId);

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status);
            }

            var totalItemCount = await query.CountAsync();

            var orders = await query
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
                    Status = o.Status, // ép kiểu để trả về dưới dạng số
                    ShippingAddress = o.ShippingAddress,
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

        public async Task<Pagination<OrderResultDTO>> GetOrdersByCustomerNameAsync(string customerName, int pageIndex, int pageSize)
        {
            var query = _context.Order
                .Where(o => o.Customer.AccountProfile.Fullname.Contains(customerName)) // Lọc theo tên khách hàng
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.CreatedAt);

            var totalItemCount = await query.CountAsync();

            var orders = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderResultDTO
                {
                    TotalPrice = o.TotalPrice,
                    Email = o.Customer.Email,
                    Status = o.Status,
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

        public Task<List<Order>> SearchOrdersByEmailAsync(string email)
        {
            var result = _context.Order
                .Where(o => o.Customer.Email.Contains(email))
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .ToListAsync();
            return result;
        }

        public Task<List<Order>> SearchOrdersByDateAsync(DateOnly date)
        {
            var result = _context.Order
                .Where(o => o.CreatedAt == date)
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .ToListAsync();
            return result;
        }
    }
}
