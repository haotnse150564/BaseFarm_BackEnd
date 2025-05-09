using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implement
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) => _context = context;

        // Đếm tổng số sản phẩm
        public async Task<int> CountAsync()
        {
            return await _context.Product.CountAsync();
        }

        public async Task<IEnumerable<Product>> GetPagedAsync(int pageIndex, int pageSize)
        {
            return await _context.Product
                .OrderBy(p => p.ProductId) // Thay đổi sắp xếp theo nhu cầu
                .Skip((pageIndex - 1) * pageSize) // Điều chỉnh Skip để trang đầu là 1
                .Take(pageSize)
                .ToListAsync();
        }

        // Đếm tổng số sản phẩm theo tên
        public async Task<int> CountByNameAsync(string productName)
        {
            return await _context.Product
                .Where(p => p.ProductName.Contains(productName))
                .CountAsync();
        }

        // Lấy danh sách sản phẩm theo tên có phân trang
        public async Task<List<Product>> GetPagedByNameAsync(string productName, int pageIndex, int pageSize)
        {
            return await _context.Product
                .Where(p => p.ProductName.Contains(productName))
                .OrderBy(p => p.ProductName) // Sắp xếp theo tên sản phẩm
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Product?> GetProductById(long productId)
        {
            return await _context.Product
                .Include(u => u.Category)
                .FirstOrDefaultAsync(u => u.ProductId == productId);
        }

        public async Task<List<Product?>> GetProductByNameAsync(string productName)
        {
            return await _context.Product
                .Where(u => u.ProductName.ToLower().StartsWith(productName.ToLower()))
                .ToListAsync(); // Trả về danh sách
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Product.AnyAsync(u => u.ProductName.ToLower() == name.ToLower());
        }
    }
}
