using Application.Commons;
using Domain.Enum;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .Include(x => x.ProductNavigation)
                .Include(X => X.Category)
                .OrderBy(p => p.ProductId) // Thay đổi sắp xếp theo nhu cầu
                .Skip((pageIndex - 1) * pageSize) // Điều chỉnh Skip để trang đầu là 1
                .Take(pageSize)
                .ToListAsync();
        }

        // Đếm tổng số sản phẩm theo tên
        public async Task<int> CountByNameAsync(string productName)
        {
            return await _context.Product
                .Include(x => x.ProductNavigation)
                .Where(p => p.ProductName.Contains(productName))
                .CountAsync();
        }

        // Lấy danh sách sản phẩm theo tên có phân trang
        public async Task<List<Product>> GetPagedByNameAsync(string productName, int pageIndex, int pageSize)
        {
            return await _context.Product
                .Include(x => x.ProductNavigation)
                .Include(X => X.Category)
                .Where(p => p.ProductName.Contains(productName))
                .OrderBy(p => p.ProductName) // Sắp xếp theo tên sản phẩm
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Product?> GetProductById(long productId)
        {
            return await _context.Product
                .Include(x => x.ProductNavigation)
                .Include(u => u.Category)
                .FirstOrDefaultAsync(u => u.ProductId == productId);
        }

        public async Task<List<Product?>> GetProductByNameAsync(string productName)
        {
            return await _context.Product
                .Include(x => x.ProductNavigation)
                .Include(u => u.Category)
                .Where(u => u.ProductName.ToLower().StartsWith(productName.ToLower()))
                .ToListAsync(); // Trả về danh sách
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Product.AnyAsync(u => u.ProductName.ToLower() == name.ToLower());
        }

        public async Task<Pagination<Product>> GetFilteredProductsAsync(int pageIndex, int pageSize, Status? status = null, long? categoryId = null, bool sortByStockAsc = true)
        {
            var query = _dbSet
                .Include(x => x.ProductNavigation)
                .Include(u => u.Category)
                .AsQueryable();

            if (status.HasValue)
                query = query/*.Include(x => x.ProductNavigation)*/.Where(p => (Status?)p.Status == status.Value);

            if (categoryId.HasValue)
                query = query/*.Include(x => x.ProductNavigation)*/.Where(p => p.CategoryId == categoryId.Value);

            query = sortByStockAsc
                ? query.OrderBy(p => p.StockQuantity)
                : query.OrderByDescending(p => p.StockQuantity);

            var itemCount = await query.CountAsync();

            if (itemCount == 0)
            {
                return new Pagination<Product>()
                {
                    PageSize = pageSize,
                    TotalItemCount = 0,
                    PageIndex = pageIndex,
                    Items = new List<Product>()
                };
            }

            var totalPages = (int)Math.Ceiling(itemCount / (double)pageSize);
            pageIndex = Math.Clamp(pageIndex, 1, totalPages);

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new Pagination<Product>
            {
                PageSize = pageSize,
                TotalItemCount = itemCount,
                PageIndex = pageIndex,
                Items = items
            };
        }
        public override async Task<List<Product>> GetAllAsync()
        {
            return await _context.Product
                .Include(x => x.ProductNavigation)
                .Include(X => X.Category)
                .ToListAsync();
        }


        /// <summary>
        /// Trừ tồn kho ở từng đợt Inventory theo FIFO và cập nhật lại Product.StockQuantity
        /// </summary>
        public async Task UpdateStockByOrderAsync(long productId, int quantityToReduce)
        {
            // Lấy các Inventory ACTIVE, theo FIFO (CreatedAt tăng dần)
            var inventories = await _context.Inventorie
                .Where(i => i.ProductId == productId && i.Status == Status.ACTIVE && i.StockQuantity > 0)
                .OrderBy(i => i.CreatedAt)
                .ToListAsync();

            int remaining = quantityToReduce;
            foreach (var inventory in inventories)
            {
                if (remaining <= 0) break;

                if (inventory.StockQuantity >= remaining)
                {
                    inventory.StockQuantity -= remaining;
                    if (inventory.StockQuantity == 0)
                        inventory.Status = Status.DEACTIVATED; // Nếu hết hàng thì chuyển trạng thái
                    remaining = 0;
                }
                else
                {
                    remaining -= inventory.StockQuantity ?? 0;
                    inventory.StockQuantity = 0;
                    inventory.Status = Status.DEACTIVATED;
                }
                _context.Inventorie.Update(inventory);
            }

            // Sau khi trừ, cập nhật lại tổng tồn kho vào Product
            var totalStock = await _context.Inventorie
                .Where(i => i.ProductId == productId && i.Status == Status.ACTIVE)
                .SumAsync(i => i.StockQuantity ?? 0);

            var product = await _context.Product.FindAsync(productId);
            if (product != null)
            {
                product.StockQuantity = totalStock;
                _context.Product.Update(product);
            }

            await _context.SaveChangesAsync();
        }
    }
}
