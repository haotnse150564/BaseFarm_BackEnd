using Domain.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implement
{
    public class FarmActivityRepository : GenericRepository<FarmActivity>, IFarmActivityRepository
    {
        public FarmActivityRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<FarmActivity>();
        }

        public async Task<List<FarmActivity>> GetAllActive()
        {
            var result = await _context.FarmActivity
                .Where(fa => fa.Status != Domain.Enum.Status.DEACTIVATED)
                .ToListAsync();
            return result;
        }
    }
}
