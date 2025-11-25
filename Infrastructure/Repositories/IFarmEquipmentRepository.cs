using Application.Repositories;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IFarmEquipmentRepository : IGenericRepository<FarmEquipment>
    {
        Task<List<FarmEquipment>> GetFarmEquipmentActive();
        Task<List<FarmEquipment>> GetFarmEquipmentByDeviceName(string deviceName);
    }
}
