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
                                     //.Include(a => a.FarmActivity)
                                     .Include(a => a.Crop)
                                     .Include(a => a.FarmDetails)
                                    // .Include(a => a.DailyLogs)
                                     .FirstOrDefaultAsync();
            return result;
        }
        public override async Task<List<Schedule>> GetAllAsync()
        {
            var result = await _context.Schedule // Thêm _context.Schedules vào đây
            .Include(a => a.AssignedToNavigation)
            //.Include(a => a.FarmActivity)
            .Include(a => a.Crop)
            .Include(a => a.FarmDetails)
           // .Include(a => a.DailyLogs)
            .ToListAsync();
            return result;
        }
    }
}
