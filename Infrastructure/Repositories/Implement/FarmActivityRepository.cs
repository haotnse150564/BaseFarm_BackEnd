using Domain.Model;
using System;
using System.Collections.Generic;
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
    }
}
