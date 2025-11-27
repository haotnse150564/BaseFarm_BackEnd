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
        [HttpPut("update-status/{cropId}")]
        public async Task<IActionResult> UpdateStatus(long cropId)
        {
            var result = await _cropRequirementServices.UpdateStatus(cropId);
            return Ok(result);
        }
        [HttpPut("update-plant-stage/{cropId}")]
        public async Task<IActionResult> UpdatePlantStage(long cropId, [FromQuery] Domain.Enum.PlantStage plantStage)
        {
            var result = await _cropRequirementServices.UpdatePlantStage(cropId, plantStage);
            return Ok(result);
        }
        [HttpPost("duplicate/{cropRequirementId}")]
        public async Task<IActionResult> DuplicateCropRequirement(long cropRequirementId, [FromQuery] Domain.Enum.PlantStage plantStage, [FromQuery] long cropId)
        {
            var result = await _cropRequirementServices.DuplicateCropRequirementAsynce(cropRequirementId, plantStage, cropId);
            return Ok(result);
        }
        [HttpGet("get-by-id/{cropId}")]
        public async Task<IActionResult> GetCropRequirementById(long cropId)
        {
            var result = await _cropRequirementServices.GetCropRequirementByIdAsync(cropId);
            return Ok(result);
        }
        [HttpPost("create/{cropId}")]
        public async Task<IActionResult> CreateCropRequirement(long cropId, [FromBody] Infrastructure.ViewModel.Request.CropRequirementRequest cropRequirement, [FromQuery] Domain.Enum.PlantStage plantStage)
        {
            var result = await _cropRequirementServices.CreateCropRequirementAsync(cropRequirement, plantStage, cropId);
            return Ok(result);
        }
        [HttpPut("update/{cropId}")]
        public async Task<IActionResult> UpdateCropRequirement(long cropId, [FromBody] Infrastructure.ViewModel.Request.CropRequirementRequest cropRequirement, [FromQuery] Domain.Enum.PlantStage plantStage)
        {
            var result = await _cropRequirementServices.UpdateCropRequirementAsync(cropRequirement, plantStage, cropId);
            return Ok(result);
        }
    }
}
