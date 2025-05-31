using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implement
{
    public class CropRequirementtRepository : GenericRepository<CropRequirement>, ICropRequirementRepository
    {
        public CropRequirementtRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<CropRequirement>();
        }
    }
    {
        
    }
}
