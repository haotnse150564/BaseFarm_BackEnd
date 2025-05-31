using Domain.Model;
using Microsoft.EntityFrameworkCore;

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
                .Where(fa => fa.Status != Domain.Enum.FarmActivityStatus.DEACTIVATED)
                .ToListAsync();
            return result;
        }

        public async Task<FarmActivity> GetHarvestFarmActivityId(long scheduleId)
        {
            return await _context.FarmActivity
                .Where(fa => fa.ScheduleId == scheduleId && fa.ActivityType == Domain.Enum.ActivityType.Harvesting && fa.Status == Domain.Enum.FarmActivityStatus.COMPLETED)
                .FirstOrDefaultAsync();
        }

        public async Task<List<FarmActivity>> GetListFarmActivityByScheduleId(long scheduleId)
        {
            return await _context.FarmActivity
                 .Where(fa => fa.ScheduleId == scheduleId)
                 .ToListAsync();
        }

        public Task<List<FarmActivity>> GetListFarmActivityUpdate(IEnumerable<long>? farmActivityId)
        {
            var result = _context.FarmActivity
                .Where(fa => farmActivityId.Contains(fa.FarmActivitiesId))
                .ToListAsync();
            return result;
        }
    }
}
