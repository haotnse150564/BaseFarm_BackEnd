using Application.Services;
using Application.ViewModel.Request;
using Domain.Enum;
using Infrastructure.ViewModel.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/crop")]
    [ApiController]
    public class CropController : ControllerBase
    {
        private readonly ICropServices _cropServices;
        public CropController(ICropServices cropServices)
        {
            _cropServices = cropServices;
        }
        [HttpGet("get-all")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllCrops(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _cropServices.GetAllCropsAsync(pageIndex, pageSize);
            return Ok(result);
        }
        [HttpGet("get-all-active")]
        [Authorize]
        public async Task<IActionResult> GetAllCropsActive()
        {
            var result = await _cropServices.GetAllCropsActiveAsync();
            return Ok(result);
        }
        [HttpPost("create")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateCrop([FromBody] Crop_Product data)
        {
            var request1 = data.request1;
            var request2 = data.request2;
            var result = await _cropServices.CreateCropAsync(request1, request2);
            return Ok(result);
        }
        [HttpPut("chang-status")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ChangeStatus(long cropId, int status)
        {
            var result = await _cropServices.UpdateCropStatusAsync(cropId, status);
            return Ok(result);
        }
        [HttpPost("search")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> SearchCrop(string? cropName, Status? status, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _cropServices.SearchCrop(cropName, status, pageIndex, pageSize);
            return Ok(result);
        }
        [HttpPut("update/{cropId}")]
        public async Task<IActionResult> UpdateCrop([FromBody] CropRequest cropUpdate, long cropId)
        {
            var result = await _cropServices.UpdateCrop(cropUpdate, cropId);
            return Ok(result);
        }
        [HttpGet("get-excluding-inactive")]
        public async Task<IActionResult> GetCropExcludingInative()
        {
            var result = await _cropServices.GetCropExcludingInativeAsync();
            return Ok(result);
        }



        //mới
        [HttpGet("new_all")]
        public async Task<IActionResult> Get_AllCrops(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _cropServices.Get_AllCropsAsync(pageIndex, pageSize);
            return Ok(result);
        }

        // 2. Get all active crops
        [HttpGet("new_active")]
        public async Task<IActionResult> Get_AllCropsActive()
        {
            var result = await _cropServices.Get_AllCropsActiveAsync();
            return Ok(result);
        }

        // 3. Search crops by name or status
        [HttpGet("new_search")]
        public async Task<IActionResult> Search_Crop(string? cropName, CropStatus? status, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _cropServices.Search_Crop(cropName, status, pageIndex, pageSize);
            return Ok(result);
        }

        // 4. Get crops excluding inactive
        [HttpGet("new-excluding-inactive")]
        public async Task<IActionResult> GetCropExcludingInactive()
        {
            var result = await _cropServices.GetCropExcludingInactiveAsync();
            return Ok(result);
        }

    }
}
