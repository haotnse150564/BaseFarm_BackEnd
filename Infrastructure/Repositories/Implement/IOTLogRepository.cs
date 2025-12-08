using Domain.Model;
using Microsoft.EntityFrameworkCore;

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
        public override async Task<List<IOTLog>> GetAllAsync()
        {
            return await _dbSet.Include(d => d.Device).ToListAsync();
        }
    }
}
