using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implement
{
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        private readonly AppDbContext _context;

        public OrderDetailRepository(AppDbContext context) => _context = context;

        public async Task<List<OrderDetail>> GetOrderDetailsByOrderId(long orderId)
        {
            return await _context.OrderDetail
                .Where(od => od.OrderId == orderId)
                .Include(od => od.Product) // Load luôn thông tin sản phẩm
                .ToListAsync();
        }
    }
}
