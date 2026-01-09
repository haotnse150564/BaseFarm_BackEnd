using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace Infrastructure.Repositories.Implement
{
    public class DevicesRepository : GenericRepository<Device>, IDevicesRepository
    {
        public DevicesRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Device>();
        }

        public async Task<Device> GetDeviceByPin(string pin)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Pin.ToLower().Equals(pin.ToLower()));

        }

        public async Task<Device> GetDevieByName(string name)
        {
            return await _dbSet
                 .Include(x => x.FarmEquipments)
                .ThenInclude(y => y.Farm)
                .FirstOrDefaultAsync(x => x.DeviceName.ToLower().Contains( name.ToLower()));
        }
        public override async Task<List<Device>> GetAllAsync()
        {
            return await _dbSet
                .Include(x => x.FarmEquipments)
                .ThenInclude(y => y.Farm)
                .ToListAsync();
        }
        public override async Task<Device?> GetByIdAsync(long id)
        {
            return await _dbSet
                .Include(x => x.FarmEquipments)
                .ThenInclude(y => y.Farm)
                .FirstOrDefaultAsync(x => x.DevicesId.Equals(id));
        }
    }
}