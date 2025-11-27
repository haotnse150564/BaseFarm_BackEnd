using Domain.Model;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<CropRequirement>> GetByCropIdAsynce(long cropId)
        {
            return await _dbSet.Where(cr => cr.CropId == cropId).ToListAsync();
        }
    }
}
