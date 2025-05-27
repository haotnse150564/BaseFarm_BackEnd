using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implement
{
    public class FeedbackRepository : GenericRepository<Feedback>, IFeedbackRepository
    {
        private readonly AppDbContext _context;

        public FeedbackRepository(AppDbContext context) => _context = context;

        // Đếm tổng số sản phẩm
        public async Task<int> CountAsync()
        {
            return await _context.Product.CountAsync();
        }

        public async Task<IEnumerable<Feedback>> GetPagedAsync(int pageIndex, int pageSize)
        {
            return await _context.Feedback
                .Include(f => f.Customer) // Include Account
                .ThenInclude(a => a.AccountProfile) // Include AccountProfile để lấy Email
                .Include(f => f.OrderDetail) 
                .ThenInclude(od => od.Product) 
                .OrderBy(f => f.FeedbackId) // Sắp xếp theo ID hoặc có thể thay đổi theo nhu cầu
                .Skip((pageIndex - 1) * pageSize) // Skip các bản ghi trước đó
                .Take(pageSize) // Lấy số bản ghi theo pageSize
                .ToListAsync();
        }

        public override async Task<Feedback> GetByIdAsync(long id)
        {
            return await _context.Feedback
                .Include(f => f.Customer) 
                .ThenInclude(a => a.AccountProfile) 
                .Include(f => f.OrderDetail)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);
        }
    }
}
