using Application.Commons;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implement
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) => _context = context;

        public async Task<Pagination<Order>> GetPagedOrdersAsync(int pageIndex, int pageSize)
        {
            var query = _context.Order
                .Include(o => o.Customer).ThenInclude(a => a.AccountProfile)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .AsQueryable();

            int totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Pagination<Order>
            {
                TotalItemCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = orders
            };
        }
    }
}
