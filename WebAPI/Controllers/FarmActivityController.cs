using Application.Services;
using Domain.Enum;
using Infrastructure.ViewModel.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/farm-activity")]
    [ApiController]
    public class FarmActivityController : ControllerBase
    {
        private readonly IFarmActivityServices _farmActivityServices;
        public FarmActivityController(IFarmActivityServices farmActivityServices)
        {
            _farmActivityServices = farmActivityServices;
        }
        [HttpGet("get-all")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetFarmActivities()
        {
            var result = await _farmActivityServices.GetFarmActivitiesAsync();
            return Ok(result);
        }
        [HttpGet("get-active")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetFarmActivitiesActive()
        {
            var result = await _farmActivityServices.GetFarmActivitiesActiveAsync();
            return Ok(result);
        }
        [HttpPost("create")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateFarmActivity([FromBody] FarmActivityRequest farmActivityRequest, ActivityType activityType)
        {
            var result = await _farmActivityServices.CreateFarmActivityAsync(farmActivityRequest, activityType);
            return Ok(result);
        }
        [HttpPut("update/{farmActivityId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateFarmActivity(long farmActivityId, [FromBody] FarmActivityRequest farmActivityRequest, ActivityType activityType)
        {
            var result = await _farmActivityServices.UpdateFarmActivityAsync(farmActivityId, farmActivityRequest, activityType);
            return Ok(result);
        }
        [HttpGet("get-by-id/{farmActivityId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetFarmActivityById(long farmActivityId)
        {
            var result = await _farmActivityServices.GetFarmActivityByIdAsync(farmActivityId);
            return Ok(result);
        }
        [HttpPut("change-status/{farmActivityId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ChangeFarmActivityStatus(long farmActivityId)
        {
            var result = await _farmActivityServices.ChangeFarmActivityStatusAsync(farmActivityId);
            return Ok(result);
        }
    }
}
