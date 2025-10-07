using Domain.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var result = await _context.Address.Where(x => x.CustomerID.Equals(userId)).ToListAsync();
            return result;
        }
    }
}
