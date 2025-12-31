using Application;
using Application.Services;
using Application.Services.Implement;
using Application.Utils;
using Infrastructure.ViewModel.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.ViewModel.Response.OrderResponse;

namespace WebAPI.Controllers
{
    [Route("api/v1/schedule-log")]
    [ApiController]
    public class ScheduleLogController : ControllerBase
    {
        private readonly IScheduleLogServices _scheduleLogServices;

        public ScheduleLogController(IScheduleLogServices scheduleLogServices)
        {
            _scheduleLogServices = scheduleLogServices;
        }

        [HttpGet("get-all-log-by-schedule/{scheduleId}")]
        //[Authorize(Roles = "Manager,Staff")]
        public async Task<ActionResult> GetScheduleLogs(long scheduleId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _scheduleLogServices.GetLogsByScheduleIdAsync(scheduleId);
            return Ok(result);
        }

        [HttpPost("create-log")]
        //[Authorize(Roles = "Manager,Staff")]
        public async Task<ActionResult> CreateManualLog([FromBody] CreateScheduleLogRequest request)
        {
            var result = await _scheduleLogServices.CreateManualLogAsync(request);
            return Ok(result);
        }

        [HttpPut("update-log")]
        //[Authorize(Roles = "Manager,Staff")]
        public async Task<ActionResult> UpdateManualLog([FromBody] UpdateScheduleLogRequest request)
        {
            var result = await _scheduleLogServices.UpdateManualLogAsync(request);

            return Ok(result);
        }
    }


}
