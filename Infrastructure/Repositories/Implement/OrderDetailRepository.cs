using Domain.Model;

namespace Infrastructure.Repositories.Implement
{
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        private readonly AppDbContext _context;

        public OrderDetailRepository(AppDbContext context) => _context = context;
    }
}
