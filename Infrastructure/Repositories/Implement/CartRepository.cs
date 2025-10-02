using Application.Repositories;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implement
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Cart>();
        }

        public Task<Cart?> GetCartByUserIdAsync(long userId)
        {
            return _context.Cart
                .Include(x => x.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == userId);
        }
    }
}
