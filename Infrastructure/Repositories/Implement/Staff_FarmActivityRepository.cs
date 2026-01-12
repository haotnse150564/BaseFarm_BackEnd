using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implement
{
    public class Staff_FarmActivityRepository : GenericRepository<Staff_FarmActivity>, IStaff_FarmActivityRepository
    {
        public Staff_FarmActivityRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Staff_FarmActivity>();
        }
    }
}
