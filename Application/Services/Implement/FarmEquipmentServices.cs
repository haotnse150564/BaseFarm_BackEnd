using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implement
{
    public class FarmEquipmentServices : IFarmEquipmentServices
    {
        public Task<object> CreateFarmEquipment(object request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFarmEquipment(long id)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<object> GetFarmEquipmentByDevicesName(long farmId)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetListEquipmentByFarmId(long id)
        {
            throw new NotImplementedException();
        }

        public Task<object> UpdateFarmEquipment(long id, object request)
        {
            throw new NotImplementedException();
        }
    }
}
