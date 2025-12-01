using Domain.Model;

namespace Infrastructure.Repositories.Implement
{
    public class IOTLogRepository : GenericRepository<IOTLog>, IIOTLogRepository
    {
        public IOTLogRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<IOTLog>();
        }

        public void DeleteRange(List<IOTLog> entities)
        {
            _context.IOTLog.RemoveRange(entities);
        }
    }
}
