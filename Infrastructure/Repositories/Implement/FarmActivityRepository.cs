using Domain.Enum;
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
                .Where(fa => fa.Status == Domain.Enum.FarmActivityStatus.ACTIVE
                          && !fa.Schedule.Any()
                          && fa.EndDate >= DateOnly.FromDateTime(DateTime.Today))
                .OrderByDescending(fa => fa.FarmActivitiesId)
                .ToListAsync();

            return result;
        }
        public async Task<List<FarmActivity>> GetAllFiler(ActivityType? type, FarmActivityStatus? status, int? month)
        {
            var list = await _context.FarmActivity.ToListAsync();
            if (type != null)
            {
                list = list.Where(fa => fa.ActivityType == type).ToList();
            }
            if (status != null)
            {
                list = list.Where(fa => fa.Status == status).ToList();
            }
            //if (month > 0 && month <= 12)
            //{
            //    list = list.Where(fa => fa.StartDate.HasValue && fa.StartDate.Value.Month == month).ToList();
            //}
            return list;
        }
        public async Task<FarmActivity> GetHarvestFarmActivityId(long scheduleId)
        {
            return await _context.FarmActivity
                //.Where(fa => fa.ScheduleId == scheduleId && fa.ActivityType == Domain.Enum.ActivityType.Harvesting && fa.Status == Domain.Enum.FarmActivityStatus.COMPLETED)
                .FirstOrDefaultAsync();
        }

        public async Task<List<FarmActivity>> GetListFarmActivityByScheduleId(long scheduleId)
        {
            return await _context.FarmActivity
                 //.Where(fa => fa.ScheduleId == scheduleId)
                 .ToListAsync();
        }

        public Task<List<FarmActivity>> GetListFarmActivityUpdate(IEnumerable<long>? farmActivityId)
        {
            var result = _context.FarmActivity
                .Where(fa => farmActivityId.Contains(fa.FarmActivitiesId))
                .ToListAsync();
            return result;
        }

        public async Task<List<Product>> GetProductWillHarves(long acitivityId)
        {
            var products = await _context.Crops
                .Include(c => c.Product) // Include Product từ Crop
                .Include(c => c.Schedules)
                    .ThenInclude(s => s.FarmActivities)
                .Where(c => c.Schedules.Any(s => s.FarmActivities.FarmActivitiesId == acitivityId))
                .Select(c => c.Product) // Lấy Product từ mỗi Crop
                .ToListAsync();

            return products;

        }
    }
}
