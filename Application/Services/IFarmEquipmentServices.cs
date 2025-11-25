using Infrastructure.ViewModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.FarmEquipmentResponse;

namespace Application.Services
{
    public interface IFarmEquipmentServices
    {
        Task<ResponseDTO> GetAll();
        Task<ResponseDTO> GetListEquipmentActive();
        Task<ResponseDTO> CreateFarmEquipment(FarmEquipmentRequest request);
        Task<ResponseDTO> RemmoveFarmEquipment(long id);
        Task<ResponseDTO> GetFarmEquipmentByDevicesName(string name);
    }
}
