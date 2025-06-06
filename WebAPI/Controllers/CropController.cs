﻿using Application.Services;
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
        public async Task<IActionResult> CreateCrop([FromBody] CropRequest request)
        {
            var result = await _cropServices.CreateCropAsync(request);
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
    }
}
