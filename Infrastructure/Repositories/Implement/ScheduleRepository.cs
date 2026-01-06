using Domain.Enum;
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
            return await _dbSet
                .Where(s => s.ScheduleId == id)

                // FarmActivities + Staff + Profile
                .Include(s => s.FarmActivities)
                    .ThenInclude(fa => fa.AssignedToNavigation)
                        .ThenInclude(a => a.AccountProfile)

                // Farm
                .Include(s => s.FarmDetails)

                // Crop + Requirement
                .Include(s => s.Crop)
                    .ThenInclude(c => c.CropRequirement)

                .FirstOrDefaultAsync();
        }


        public override async Task<List<Schedule>> GetAllAsync()
        {
            return await _context.Schedule
                .Include(s => s.FarmActivities)
                    .ThenInclude(fa => fa.AssignedToNavigation)
                        .ThenInclude(a => a.AccountProfile)
                .Include(s => s.FarmDetails)
                .Include(s => s.Crop)
                    .ThenInclude(c => c.CropRequirement)
                .OrderByDescending(s => s.ScheduleId)
                .ToListAsync();
        }


        public async Task<List<Schedule>> GetByStaffIdAsync(long staffId, int month)
        {
            var query = _context.Schedule
                .Include(s => s.AssignedToNavigation)
                    .ThenInclude(a => a.AccountProfile)
                .Include(s => s.Crop)
                    .ThenInclude(c => c.CropRequirement)
                .Include(s => s.FarmDetails)
                .Include(s => s.FarmActivities)
                    .ThenInclude(fa => fa.AssignedToNavigation)
                        .ThenInclude(a => a.AccountProfile)
                // CHỈ LẤY schedule có activity của staff
                .Where(s => s.FarmActivities.Any(fa => fa.AssignedTo == staffId))
                .OrderByDescending(s => s.ScheduleId);

            if (month >= 1 && month <= 12)
            {
                query = (IOrderedQueryable<Schedule>)query.Where(s =>
                    s.StartDate.HasValue &&
                    s.StartDate.Value.Month == month
                );
            }

            return await query.ToListAsync();
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
            //.Where(x => x.AssignedTo == staffId && x.Status == Domain.Enum.Status.ACTIVE)
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

        public async Task<List<Schedule>> GetAllActiveScheduleAsync()
        {
            var result = await _context.Schedule
            .Include(s => s.AssignedToNavigation).ThenInclude(ap => ap.AccountProfile)
            .Include(f => f.FarmDetails)
            .Include(c => c.Crop).ThenInclude(c => c.CropRequirement)
            .Include(fa => fa.FarmActivities)
            .OrderByDescending(x => x.ScheduleId)
            .Where(s => s.Status == Status.ACTIVE)
            .ToListAsync();
            return result;
        }
    }
}
