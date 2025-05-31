using Application;
using Application.Services;
using Infrastructure.ViewModel.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var result = await _schedule.CreateScheduleAsync(request);

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
            var result = await _schedule.AssignStaff(scheduleId, staffId);

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
            var result = await _schedule.GetAllScheduleAsync(pageIndex, pageSize);

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
            var result = await _schedule.GetScheduleByIdAsync(id);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
        [HttpPut("schedule-update-status")]
        [Authorize(Roles = "Manager")]

        public async Task<IActionResult> UpdateScheduleStatus(long scheduleId,[FromBody] string status)
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
            var result = await _schedule.UpdateScheduleById(scheduleId, request);

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
            var result = await _schedule.GetScheduleByCurrentStaffAsync(month);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); 
            }

            return Ok(result); 
        }
    }
}
