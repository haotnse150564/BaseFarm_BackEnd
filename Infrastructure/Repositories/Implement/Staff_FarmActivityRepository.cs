using Domain.Enum;
using Domain.Model;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories.Implement
{
    public class Staff_FarmActivityRepository : GenericRepository<Staff_FarmActivity>, IStaff_FarmActivityRepository
    {
        public Staff_FarmActivityRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Staff_FarmActivity>();
        }
        public override async Task<List<Staff_FarmActivity>> GetAllAsync()
        {
            return await _dbSet.Include(x => x.Account)
                         .ThenInclude(x => x.AccountProfile)
                         .Include(x => x.FarmActivity)
                         .ThenInclude(x => x.Schedule)
                         .ThenInclude(x => x.Crop)
                         .OrderByDescending(x => x.Staff_FarmActivityId)
                         .ToListAsync();
        }

        public Task<List<Staff_FarmActivity>> GetByFarmActivityIdAsync(long ActivityId)
        {
            return _dbSet.Where(x => x.FarmActivityId == ActivityId)
                         .Include(x => x.Account)
                         .ThenInclude(x => x.AccountProfile)
                         .Include(x => x.FarmActivity)
                         .ThenInclude(x => x.Schedule)
                         .ThenInclude(x => x.Crop)
                         .OrderByDescending(x => x.Staff_FarmActivityId)
                         .ToListAsync();
        }

        public override async Task<Staff_FarmActivity?> GetByIdAsync(long id)
        {
            return await _dbSet.Include(x => x.Account)
                         .ThenInclude(x => x.AccountProfile)
                         .Include(x => x.FarmActivity)
                         .ThenInclude(x => x.Schedule)
                         .ThenInclude(x => x.Crop)
                         .Where(x => x.Staff_FarmActivityId == id)
                         .FirstOrDefaultAsync();
        }


        public Task<List<Staff_FarmActivity>> GetByStaffIdAsync(long staffId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> HasStaffTimeConflictAsync(long staffId,DateOnly startDate,DateOnly endDate,long? excludeStaffFarmActivityId = null)
        {
            var query = _context.StaffFarmActivities
                .Include(sfa => sfa.FarmActivity)  // cần join với FarmActivity để lấy Start/EndDate
                .Where(sfa =>
                    sfa.AccountId == staffId &&                           // Chỉ nhân viên này
                    (excludeStaffFarmActivityId == null ||
                     sfa.Staff_FarmActivityId != excludeStaffFarmActivityId) &&  // Không tính bản ghi đang sửa
                    sfa.FarmActivity != null &&
                    sfa.FarmActivity.Status != FarmActivityStatus.DEACTIVATED &&
                    sfa.FarmActivity.Status != FarmActivityStatus.DEACTIVATED && 
                    sfa.FarmActivity.Schedule != null &&
                    sfa.FarmActivity.Schedule.Status == Status.ACTIVE &&   // Chỉ hoạt động thuộc lịch ACTIVE
                    sfa.FarmActivity.StartDate <= endDate &&               // Chồng chéo: mới bắt đầu trước cũ kết thúc
                    sfa.FarmActivity.EndDate >= startDate                  // cũ bắt đầu trước mới kết thúc
                );

            return await query.AnyAsync();
        }
    }
}
