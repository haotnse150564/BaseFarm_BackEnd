using Domain;
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

        public async Task<List<Product?>> getAllProductAsync()
        {
            return await _context.Product.ToListAsync();
        }

        public async Task<Product?> GetProductByCurrentId(int productId)
        {
            return await _context.Product
                .Include(u => u.Category)
                .FirstOrDefaultAsync(u => u.ProductId == productId);  // Tìm user theo userId
        }
    }
}
