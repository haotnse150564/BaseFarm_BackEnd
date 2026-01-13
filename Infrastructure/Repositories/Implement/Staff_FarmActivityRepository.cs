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
    }
}
