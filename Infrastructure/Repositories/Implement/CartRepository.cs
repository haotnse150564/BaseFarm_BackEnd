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

        public async Task<Cart?> GetCartByUserIdAsync(long userId)
        {
            var ressult =  await _context.Cart
                .Include(x => x.CartItems)
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(c => c.CustomerId == userId);
            return ressult;
        }
    }
}
