using Domain.Model;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories.Implement
{
    public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Schedule>();
        }
        public override async Task<Schedule?> GetByIdAsync(long id)
        {
            var result = await _dbSet.Where(x => x.ScheduleId == id)
                                     .Include(a => a.AssignedToNavigation)
                                     .Include(a => a.FarmActivities)
                                     .Include(a => a.Crop)
                                     .ThenInclude(c => c.Product)
                                     .Include(a => a.FarmDetails)
                                    // .Include(a => a.DailyLogs)
                                     .FirstOrDefaultAsync();
            return result;
        }
        public override async Task<List<Schedule>> GetAllAsync()
        {
            var result = await _context.Schedule // Thêm _context.Schedules vào đây
            .Include(a => a.AssignedToNavigation)
            .Include(a => a.Crop)
            .Include(a => a.FarmActivities)
            .Include(a => a.FarmDetails)
           // .Include(a => a.DailyLogs)
            .ToListAsync();
            return result;
        }

        public async Task<List<Schedule?>> GetByStaffIdAsync(long staffId, int month)
        {
            var result = await _context.Schedule
            .Include(a => a.AssignedToNavigation)
            .Include(a => a.Crop)
            .Include(a => a.AssignedToNavigation)
            .ThenInclude(a => a.AccountProfile)
            .Include(a => a.FarmActivities)
            .Include(a => a.FarmDetails)
            .Where(x => x.AssignedTo == staffId)
            .ToListAsync();
            if(month > 0 && month < 12)
            {
                result = result.Where(x => x.StartDate.HasValue && x.StartDate.Value.Month == month).ToList();
            }
            return result;
        }

        public async Task<Schedule?> GetByIdWithFarmActivitiesAsync(long scheduleId)
        {
            return await _context.Schedule
                .Include(s => s.FarmActivities)
                .Include (s => s.Crop)
                .ThenInclude(c => c.Product)
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);
        }
    }
}
