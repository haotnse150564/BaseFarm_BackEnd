using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implement
{
    public interface IFarmEquipmentServices
    {
        Task<object> GetAll();
        Task<object> GetListEquipmentByFarmId(long id);
        Task<object> CreateFarmEquipment(List<int> listId);
        Task<bool> RemmoveFarmEquipment(long id);
        Task<object> GetFarmEquipmentByDevicesName(long farmId);
    }
}
