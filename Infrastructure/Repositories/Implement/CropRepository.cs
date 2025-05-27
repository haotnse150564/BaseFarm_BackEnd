using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public override async Task<List<Crop>> GetAllAsync()
        {
            return await _context.Crops
                .Include(c => c.Category)
                .ToListAsync();
        }
    }
}

