using Domain.Enum;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using static Infrastructure.ViewModel.Response.CropRequirementResponse;
namespace Application.Services
{
    public interface ICropRequirementServices
    {
        Task<ResponseDTO> GetCropRequirementByIdAsync(long cropId);
        Task<ResponseDTO?> CreateCropRequirementAsync(CropRequirementRequest cropRequirement, PlantStage plantStage, long cropId);
        Task<ResponseDTO?> UpdateCropRequirementAsync(CropRequirementRequest cropRequirement, PlantStage plantStage, long cropId);
        Task<ResponseDTO> DeleteCropRequirementAsync(long cropRequirementId);
        Task<ResponseDTO> DuplicateCropRequirementAsynce(long cropRequirementId, PlantStage plantStage, long cropId);
        Task<ResponseDTO> UpdateStatus(long cropId);
        Task<ResponseDTO> UpdatePlantStage(long cropId, PlantStage plantStage);
        Task<ResponseDTO> GetAll();
    }
}
