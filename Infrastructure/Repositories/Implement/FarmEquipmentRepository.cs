using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implement
{
    public class FarmEquipmentRepository : GenericRepository<FarmEquipment>, IFarmEquipmentRepository
    {
        public FarmEquipmentRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<FarmEquipment>();
        }

        public async Task<List<FarmEquipment>> GetFarmEquipmentActive()
        {
            return await _dbSet
                .Include(fe => fe.Device)
                .Include(fe => fe.Farm)
                .Where(fe => fe.Status == Domain.Enum.Status.ACTIVE).ToListAsync();
        }

        public async Task<List<FarmEquipment>> GetFarmEquipmentByDeviceName(string deviceName)
        {
            return await _dbSet
                .Include(fe => fe.Device)
                .Include(fe => fe.Farm)
                .Where(fe => fe.Device.DeviceName.Contains(deviceName))
                .ToListAsync();
        }
        public override async Task<List<FarmEquipment>> GetAllAsync()
        {
            return await _dbSet
                .Include(fe => fe.Device)
                .Include(fe => fe.Farm)
                .ToListAsync();
        }
    }
}
