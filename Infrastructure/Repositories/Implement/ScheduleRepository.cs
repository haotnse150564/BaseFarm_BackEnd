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
            .Include(s => s.AssignedToNavigation).ThenInclude(ap => ap.AccountProfile)
            .Include(f => f.FarmDetails)
            .Include(c => c.Crop).ThenInclude(c => c.CropRequirement)
            .Include(fa => fa.FarmActivities)
            .FirstOrDefaultAsync();
            return result;
        }
        public override async Task<List<Schedule>> GetAllAsync()
        {
            var result = await _context.Schedule // Thêm _context.Schedules vào đây
            .Include(s => s.AssignedToNavigation).ThenInclude(ap => ap.AccountProfile)
            .Include(f => f.FarmDetails)
            .Include(c => c.Crop).ThenInclude(c => c.CropRequirement)
            .Include(fa => fa.FarmActivities)
            .OrderByDescending(x => x.ScheduleId)
            .ToListAsync();
            return result;
        }

        public async Task<List<Schedule?>> GetByStaffIdAsync(long staffId, int month)
        {
            var result = await _context.Schedule
            .Include(a => a.AssignedToNavigation)
            .Include(a => a.Crop).ThenInclude(c => c.CropRequirement)
            .Include(a => a.AssignedToNavigation)            
            .ThenInclude(a => a.AccountProfile)
            .Include(a => a.FarmActivities)
            .Include(a => a.FarmDetails)
            .Where(x => x.AssignedTo == staffId)
            .OrderByDescending(x => x.ScheduleId)
            .ToListAsync();
            if (month > 0 && month < 12)
            {
                result = result.Where(x => x.StartDate.HasValue && x.StartDate.Value.Month == month).ToList();
            }
            return result;
        }
        public async Task<List<Schedule?>> GetScheduleByStaffIdAsync(long staffId, int month)
        {
            var result = await _context.Schedule
            .Include(a => a.AssignedToNavigation)
            .Include(a => a.Crop).ThenInclude(c => c.CropRequirement)
            .Include(a => a.AssignedToNavigation)
            .ThenInclude(a => a.AccountProfile)
            .Include(a => a.FarmActivities)
            .Include(a => a.FarmDetails)
            .Where(x => x.AssignedTo == staffId && x.Status == Domain.Enum.Status.ACTIVE)
            .OrderByDescending(x => x.ScheduleId)
            .ToListAsync();
            if (month > 0 && month < 12)
            {
                result = result.Where(x => x.StartDate.HasValue && x.StartDate.Value.Month == month).ToList();
            }
            return result;
        }
        public async Task<Schedule?> GetByIdWithFarmActivitiesAsync(long scheduleId)
        {
            return await _context.Schedule
            //    .Include(s => s.FarmActivities)
                .Include(s => s.Crop)
                .ThenInclude(c => c.Product)
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);
        }

        public async Task<Schedule?> GetByCropId(long cropId)
        {
            return await _context.Schedule
                .Include(s => s.Crop)
                .ThenInclude(c => c.CropRequirement)
                .FirstOrDefaultAsync(x => x.CropId == cropId);
        }

        public async Task<Schedule?> GetByIdWithCropRequirementsAsync(long scheduleId, long managerId)
        {
            return await _dbSet
                .Include(s => s.Crop)
                    .ThenInclude(c => c.CropRequirement.Where(cr => cr.IsActive)) // chỉ lấy active, tối ưu
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId && s.ManagerId == managerId);
        }
    }
}
