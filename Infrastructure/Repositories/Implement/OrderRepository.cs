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

        public async Task<Pagination<OrderResultDTO>> GetAllOrdersAsync(int pageIndex, int pageSize, PaymentStatus? status)
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
                .OrderByDescending(o => o.OrderId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderResultDTO
                {
                    OrderId = o.OrderId,
                    TotalPrice = o.TotalPrice,
                    Email = o.Customer.Email,
                    ShippingAddress = o.ShippingAddress,
                    Status = (Status?)o.Status,
                    CreatedAt = o.CreatedAt.HasValue ? o.CreatedAt.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    OrderItems = o.OrderDetails.Select(od => new ViewProductDTO
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Product.ProductName,
                        Price = od.UnitPrice,
                        StockQuantity = od.Quantity,
                        Images = od.Product.Images
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


        public async Task<Pagination<OrderResultDTO>> GetOrdersByCustomerIdAsync(long customerId, int pageIndex, int pageSize, PaymentStatus? status)
        {
            var query = _context.Order
                .Where(o => o.CustomerId == customerId);

            if (status.HasValue)
            {
                query = query.Where(o => (PaymentStatus?)o.Status == status);
            }

            var totalItemCount = await query.CountAsync();

            var orders = await query
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderResultDTO
                {
                    OrderId = o.OrderId,
                    TotalPrice = o.TotalPrice,
                    Email = o.Customer.Email,
                    Status = (Status?)o.Status, // ép kiểu để trả về dưới dạng số
                    ShippingAddress = o.ShippingAddress,
                    OrderDetailIds = o.OrderDetails.Select(od => od.OrderDetailId).ToList(),
                    CreatedAt = o.CreatedAt.HasValue ? o.CreatedAt.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    OrderItems = o.OrderDetails.Select(od => new ViewProductDTO
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Product.ProductName,
                        Images = od.Product.Images,
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
                .OrderByDescending(o => o.OrderId) // Sắp xếp theo ngày mới nhất
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
                .OrderByDescending(o => o.OrderId);

            var totalItemCount = await query.CountAsync();

            var orders = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderResultDTO
                {
                    TotalPrice = o.TotalPrice,
                    Email = o.Customer.Email,
                    Status = (Status?)o.Status,
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

        public Task<List<Order>> SearchOrdersByEmailAsync(string email, PaymentStatus? status)
        {

            if (status.HasValue)
            {
                var result = _context.Order
                     .Where(o => o.Customer.Email.Contains(email) && o.Status == status)
                     .Include(o => o.Customer)
                     .Include(o => o.OrderDetails)
                         .ThenInclude(od => od.Product)
                         .OrderByDescending(o => o.OrderId)
                     .ToListAsync();
                return result;
            }
            var results = _context.Order
                .Where(o => o.Customer.Email.Contains(email))
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .ToListAsync();
            return results;
        }

        public Task<List<Order>> SearchOrdersByDateAsync(DateOnly date)
        {
            var result = _context.Order
                .Where(o => o.CreatedAt == date)
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .OrderByDescending(o => o.OrderId)
                .ToListAsync();
            return result;
        }
    }
}
