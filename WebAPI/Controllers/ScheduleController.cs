using Application;
using Application.Services;
using Application.Services.Implement;
using Infrastructure.ViewModel.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using static Infrastructure.ViewModel.Response.ScheduleResponse;

namespace WebAPI.Controllers
{

    [Route("api/v1/Schedule")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleServices _schedule;
        private readonly ILogger<OrderController> _logger;

        public ScheduleController(IScheduleServices schedule, ILogger<OrderController> logger)
        {
            _schedule = schedule;

            _logger = logger;
        }
        [HttpPost("schedule-create")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateSchedule([FromBody] ScheduleRequest request)
        {
            var result = await _schedule.CreateSchedulesAsync(request);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
        [HttpPut("schedule-assign-staff")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AssidnStaff(long scheduleId, [FromBody] long staffId)
        {
            var result = await _schedule.AssignTask(scheduleId, staffId);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
        [HttpGet("schedule-list")]
        [Authorize(Roles = "Manager,Staff")]
        public async Task<IActionResult> GetListSchedule([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _schedule.GetAllSchedulesAsync(pageIndex, pageSize);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
        [HttpGet("schedule-byId")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetScheduleByID(long id)
        {
            var result = await _schedule.ScheduleByIdAsync(id);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
        [HttpPut("schedule-update-status")]
        [Authorize(Roles = "Manager")]

        public async Task<IActionResult> UpdateScheduleStatus(long scheduleId, [FromBody] string status)
        {
            var result = await _schedule.ChangeScheduleStatusById(scheduleId, status.ToUpper());

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
        [HttpPut("schedule-update")]
        [Authorize(Roles = "Manager")]

        public async Task<IActionResult> UpdateSchedule(long scheduleId, [FromBody] ScheduleRequest request)
        {
            var result = await _schedule.UpdateSchedulesAsync(scheduleId, request);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }

        [HttpGet("schedule-by-staff")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetScheduleByStaffID(int month)
        {
            var result = await _schedule.ScheduleStaffView(month);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpPut("schedule-update-activities")]
        [Authorize(Roles = "Manager,Staff")]
        public async Task<IActionResult> UpdateActivity(long scheduleId, long[] activityIds)
        {
            var result = await _schedule.UpdateActivities(scheduleId, activityIds);
            if (result.Status != Const.FAIL_UPDATE_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }
            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }

        /// <summary>
        /// Cập nhật ngày hiện tại cho Schedule (dùng để tính stage tự động)
        /// </summary>
        /// <param name="scheduleId">ID của Schedule</param>
        /// <param name="request">CustomToday = null → lấy ngày hiện tại; có giá trị → dùng để demo/backdate</param>
        [HttpPut("update-today/{scheduleId}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<UpdateTodayResponse>> UpdateToday(long scheduleId,[FromBody] UpdateTodayRequest? request)
        {
            var response = await _schedule.UpdateTodayAsync(scheduleId, request);
            return Ok(response);
        }
        //[HttpPut("add-farm-activity/{scheduleId}")]
        //[Authorize(Roles = "Manager")]
        //public async Task<IActionResult> AddFarmActivityToSchedule(long scheduleId, [FromBody] long farmActivities)
        //{
        //    var result = await _schedule.AddFarmActivityToSchedule(scheduleId, farmActivities);
        //    if (result.Status != Const.SUCCESS_READ_CODE)
        //    {
        //        return BadRequest(result); // Trả về lỗi 400 nếu thất bại
        //    }
        //    return Ok(result); // Trả về danh sách sản phẩm với phân trang
        //}
    }
}
