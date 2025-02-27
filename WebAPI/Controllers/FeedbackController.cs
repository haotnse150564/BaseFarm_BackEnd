using Application;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/feedback")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackSevices _feedbackService;

        public FeedbackController(IFeedbackSevices feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet("feedBackList")]
        public async Task<IActionResult> GetListFeedback([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _feedbackService.GetAllFeedbackAsync(pageIndex, pageSize);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
    }
}
