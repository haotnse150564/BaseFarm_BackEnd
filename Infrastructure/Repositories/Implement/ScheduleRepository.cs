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
                                     .FirstOrDefaultAsync();
            return result;
        }
    }
}
