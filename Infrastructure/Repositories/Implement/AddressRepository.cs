using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implement
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Address>();
        }

        public async Task<List<Address>> GetListAddressByUserID(long userId)
        {
            var result = await _dbSet
                .Include(a => a.Account)
                .ThenInclude(ap => ap.AccountProfile)
                .Where(x => x.CustomerID.Equals(userId))
                .ToListAsync();
            return result;
        }
        public override Task<Address?> GetByIdAsync(long id)
        {
            var result = _dbSet
                .Include(a => a.Account)
                .ThenInclude(ap => ap.AccountProfile)
                .FirstOrDefaultAsync(x => x.AddressID.Equals(id));
            return result;
        }
    }
}
