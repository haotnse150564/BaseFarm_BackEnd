using Application;
using Application.Services;
using Infrastructure.ViewModel.Request;
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
        [HttpGet("schedule-list")]
        public async Task<IActionResult> GetListSchedule([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _schedule.GetAllScheduleAsync(pageIndex, pageSize);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
        [HttpGet("schedule-update-status")]
        public async Task<IActionResult> UpdateScheduleStatus(long scheduleId, string status)
        {
            var result = await _schedule.ChangeScheduleStatusById(scheduleId, status.ToUpper());

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
        [HttpPut("schedule-update")]
        public async Task<IActionResult> UpdateSchedule(long scheduleId, ScheduleRequest request)
        {
            var result = await _schedule.UpdateScheduleById(scheduleId, request);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
    }
}
