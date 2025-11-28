using Domain.Model;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories.Implement
{
    public class DevicesRepository : GenericRepository<Device>, IDevicesRepository
    {
        public DevicesRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Device>();
        }

        public async Task<Device> GetDevieByName(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.DeviceName.ToLower().Contains( name.ToLower()));
        }
    }
}