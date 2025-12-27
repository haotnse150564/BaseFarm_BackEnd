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
                .Include(s => s.Schedules).ThenInclude(s => s.FarmActivities)
                .Include(p => p.Product)
                .Include(cr => cr.CropRequirement)
                .Include(ca => ca.Category)
                .ToListAsync();
        }

        public async Task<List<Crop>> GetAllexcludingInactiveAsync()
        {
            var result = await _context.Crops
                .Where(c => c.Status != Domain.Enum.CropStatus.INACTIVE)
                .Include(c => c.Category)
                .Include(s => s.Schedules).ThenInclude(s => s.FarmActivities)
                .Include(p => p.Product)
                .Include(cr => cr.CropRequirement)
                .ToListAsync();
            return result;
        }
        public override async Task<Crop?> GetByIdAsync(long id)
        {
            var result = await _context.Crops
                .Include(c => c.Category)
                .Include(s => s.Schedules).ThenInclude(s => s.FarmActivities)
                .Include(p => p.Product)
                .Include(cr => cr.CropRequirement)
                .FirstOrDefaultAsync(x => x.CropId == id);
            return result;
        }
    }
}

