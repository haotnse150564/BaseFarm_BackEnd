using Application;
using Application.Services;
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
    }
}
