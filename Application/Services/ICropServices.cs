using Application.Commons;
using Application.ViewModel.Request;
using Domain.Enum;
using Domain.Model;
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
        Task<ResponseDTO> CreateCropAsync(CropRequest request1, ProductRequestDTO.CreateProductDTO request2);
        Task<ResponseDTO> UpdateCropStatusAsync(long cropId, int status);
        Task<ResponseDTO> SearchCrop(string? cropName, Status? status, int pageIndex, int pageSize);
        Task<ResponseDTO> UpdateCrop(CropRequest cropUpdate, long cropId);
        Task<ResponseDTO> GetCropExcludingInativeAsync();
        Task<Pagination<CropView>> Get_AllCropsAsync(int pageIndex, int pageSize);
        Task<List<CropView>> Get_AllCropsActiveAsync();
        Task<ResponseDTO> Search_Crop(string? cropName, Status? status, int pageIndex, int pageSize);
        Task<ResponseDTO> GetCropExcludingInactiveAsync();

    }
}
