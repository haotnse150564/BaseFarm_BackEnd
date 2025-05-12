using Application.Commons;
using Domain.Enum;
using Infrastructure.ViewModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.CropResponse;

namespace Application.Services
{
    public interface ICropServices
    {
        Task<Pagination<CropView>> GetAllCropsAsync(int pageIndex, int pageSize);
        Task<List<CropView>> GetAllCropsActiveAsync();
        Task<ResponseDTO> CreateCropAsync(CropRequest request);
        Task<ResponseDTO> UpdateCropStatusAsync(long cropId);
        Task<ResponseDTO> SearchCrop(CropFilter filter, Status? status, int pageIndex, int pageSize);
        Task<ResponseDTO> UpdateCrop(CropUpdate cropUpdate, long cropId);
    }
}
