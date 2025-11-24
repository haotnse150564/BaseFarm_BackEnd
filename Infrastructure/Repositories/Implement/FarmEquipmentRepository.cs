using Domain.Model;
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
    }
}
