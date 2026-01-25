using Application.Repositories;
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
    public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context) => _context = context;

        public async Task<int> GetTotalStockByProductIdAsync(long productId)
        {
            return await _context.Inventorie
                .Where(i => i.ProductId == productId && i.Status == Status.ACTIVE)
                .SumAsync(i => i.StockQuantity ?? 0);
        }

        public async Task<Inventory?> GetByProductIdAsync(long productId)
        {
            return await _context.Inventorie
                .Where(i => i.ProductId == productId)
                .Include(i => i.Product)     
                .Include(i => i.Schedule)    
                .FirstOrDefaultAsync();
        }
    }
}
