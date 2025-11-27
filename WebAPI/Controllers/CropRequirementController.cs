using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/crop-requirement")]
    [ApiController]
    public class CropRequirementController : ControllerBase
    {
        private readonly ICropRequirementServices _cropRequirementServices;
        public CropRequirementController(ICropRequirementServices cropRequirementServices)
        {
            _cropRequirementServices = cropRequirementServices;
        }
        [HttpGet("get-by-crop-id")]
        public async Task<IActionResult> GetCropRequirementsByCropId([FromQuery] long cropId)
        {
            var result = await _cropRequirementServices.GetListCropRequirementByCropId(cropId);
            return Ok(result);
        }


        [HttpGet("get-all")]
        public async Task<IActionResult> GetCropRequirements()
        {
            var result = await _cropRequirementServices.GetAll();
            return Ok(result);
        }
        [HttpDelete("delete/{cropRequirementId}")]
        public async Task<IActionResult> DeleteCropRequirement(long cropRequirementId)
        {
            var result = await _cropRequirementServices.DeleteCropRequirementAsync(cropRequirementId);
            return Ok(result);
        }
        [HttpPut("update-status/{cropRequirementId}")]
        public async Task<IActionResult> UpdateStatus(long cropRequirementId)
        {
            var result = await _cropRequirementServices.UpdateStatus(cropRequirementId);
            return Ok(result);
        }
        [HttpPut("update-plant-stage/{cropRequirementId}")]
        public async Task<IActionResult> UpdatePlantStage(long cropRequirementId, [FromQuery] Domain.Enum.PlantStage plantStage)
        {
            var result = await _cropRequirementServices.UpdatePlantStage(cropRequirementId, plantStage);
            return Ok(result);
        }
        [HttpPost("duplicate/{cropRequirementId}")]
        public async Task<IActionResult> DuplicateCropRequirement(long cropRequirementId, [FromQuery] Domain.Enum.PlantStage plantStage, [FromQuery] long cropId)
        {
            var result = await _cropRequirementServices.DuplicateCropRequirementAsynce(cropRequirementId, plantStage, cropId);
            return Ok(result);
        }
        [HttpGet("get-by-id/{cropRequirementId}")]
        public async Task<IActionResult> GetCropRequirementById(long cropRequirementId)
        {
            var result = await _cropRequirementServices.GetCropRequirementByIdAsync(cropRequirementId);
            return Ok(result);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateCropRequirement(long cropRequirementId, [FromBody] Infrastructure.ViewModel.Request.CropRequirementRequest cropRequirement, [FromQuery] Domain.Enum.PlantStage plantStage)
        {
            var result = await _cropRequirementServices.CreateCropRequirementAsync(cropRequirement, plantStage, cropRequirementId);
            return Ok(result);
        }
        [HttpPut("update/{cropRequirementId}")]
        public async Task<IActionResult> UpdateCropRequirement(long cropRequirementId, [FromBody] Infrastructure.ViewModel.Request.CropRequirementRequest cropRequirement, [FromQuery] Domain.Enum.PlantStage plantStage)
        {
            var result = await _cropRequirementServices.UpdateCropRequirementAsync(cropRequirement, plantStage, cropRequirementId);
            return Ok(result);
        }
    }
}
