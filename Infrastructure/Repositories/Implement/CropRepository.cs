using Domain.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implement
{
    public class CropRepository : GenericRepository<Crop>, ICropRepository
    {
        public CropRepository(AppDbContext context) 
        {
            _context = context;
            _dbSet = _context.Set<Crop>();
        }

        public async Task<bool> CheckDuplicateCropName(string cropName)
        {
            return await _context.Crops.AnyAsync(u => u.CropName.ToLower() == cropName.ToLower());
        }
    }
}

