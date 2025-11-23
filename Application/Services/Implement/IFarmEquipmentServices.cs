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
        Task<object> CreateFarmEquipment(object request);
        Task<object> UpdateFarmEquipment(long id, object request);
        Task<bool> DeleteFarmEquipment(long id);
        Task<object> GetFarmEquipmentByDevicesName(long farmId);
    }
}
