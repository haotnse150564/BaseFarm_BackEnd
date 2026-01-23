using Application.Services;
using Domain.Enum;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

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
        public async Task<IActionResult> GetFarmActivities(ActivityType? type, FarmActivityStatus? status, int? month, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _farmActivityServices.GetFarmActivitiesAsync(pageIndex, pageSize, type, status, month);
            return Ok(result);
        }
        [HttpGet("get-active")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetFarmActivitiesActive([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _farmActivityServices.GetFarmActivitiesActiveAsync(pageIndex, pageSize);
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
        public async Task<IActionResult> UpdateFarmActivity(long farmActivityId, [FromBody] FarmActivityRequest farmActivityRequest, ActivityType activityType, FarmActivityStatus farmActivityStatus)
        {
            var result = await _farmActivityServices.UpdateFarmActivityAsync(farmActivityId, farmActivityRequest, activityType, farmActivityStatus);
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
        [HttpPut("complete/{id}")]
        [Authorize(Roles = "Manager, Staff")]
        public async Task<IActionResult> CompleteFarmActivity(long id, string? location)
        {
            var result = await _farmActivityServices.CompleteFarmActivity(id, location);
            return Ok(result);
        }
        [HttpGet("get-by-staff")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetFarmActivitiesByStaff(ActivityType? type, FarmActivityStatus? status, int? month, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _farmActivityServices.GetFarmActivitiesByStaffAsync(pageIndex, pageSize, type, status, month);
            return Ok(result);
        }
        [HttpPost("staff-activity/bind")]
        public async Task<IActionResult> AddStafftoFarmActivity(long farmActivityId, long staffId)
        {
            var result = await _farmActivityServices.AddStafftoFarmActivity(farmActivityId, staffId);
            return Ok(result);
        }
        [HttpPut("staff-activity/status")]
        public async Task<IActionResult> UpdateStafftoFarmActivity(long id)
        {
            var result = await _farmActivityServices.UpdateStafftoFarmActivity(id);
            return Ok(result);
        }
        [HttpGet("staff-activity/get-all")]
        public async Task<IActionResult> GetAllFarmTask()
        {
            var result = await _farmActivityServices.GetAllFarmTask();
            return Ok(result);
        }
        [HttpGet("staff-activity/get-by-id/{taskId}")]
        public async Task<IActionResult> GetFarmTaskById(long taskId)
        {
            var result = await _farmActivityServices.GetFarmTaskById(taskId);
            return Ok(result);
        }
        [HttpGet("staff-activity/get-staff/{farmActivityId}")]
        public async Task<IActionResult> GetStaffByFarmActivityId(long farmActivityId)
        {
            var result = await _farmActivityServices.GetStaffByFarmActivityId(farmActivityId);
            return Ok(result);
        }

        [HttpPost("complete-my-activity")]
        public async Task<IActionResult> ReportMyPartCompleted(long farmActivityId, string? note)
        {

            var result = await _farmActivityServices.ReportMyPartCompletedAsync(farmActivityId, note);

            return Ok(result);
        }

        [HttpGet("farm-activity-by-schedule")]
        public async Task<IActionResult> GetFarmActivityBySchedule(long scheduleId)
        {

            var result = await _farmActivityServices.GetFarmActivityByScheduleIdAsync(scheduleId);

            return Ok(result);
        }

        [HttpGet("assigned-staff-by-farm-activity")]
        public async Task<IActionResult> getAssignStaffByFarmActivityId(long farmActivityId)
        {
            var result = await _farmActivityServices.GetAssignStaffByFarmActivityId(farmActivityId);

            return Ok(result);
        }
    }
}
