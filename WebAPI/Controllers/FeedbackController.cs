using Application;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Infrastructure.ViewModel.Request.FeedbackRequest;
using static Infrastructure.ViewModel.Response.FeedbackResponse;

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

        [HttpGet("feed-back-list")]
        public async Task<IActionResult> GetListFeedback([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _feedbackService.GetAllFeedbackAsync(pageIndex, pageSize);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }

        [HttpPost("create-feedback")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackDTO createRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _feedbackService.CreateFeedbackAsync(createRequest);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("update-feedback/{id}")]
        [Authorize(Roles = "Customer,Staff")]
        public async Task<IActionResult> UpdateFeedbackAsync([FromBody] CreateFeedbackDTO request, [FromRoute] long id)
        {

            // Kiểm tra xem request có hợp lệ không
            if (request == null)
            {
                return BadRequest(new ResponseDTO(Const.FAIL_READ_CODE, "Invalid request."));
            }

            var response = await _feedbackService.UpdateFeedbackById(id, request);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }

        [HttpPost("update-feedback-status/{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> UpdateFeedbackStatusAsync([FromRoute] long id)
        {
            var response = await _feedbackService.UpdateFeedbackStatusById(id);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }
    }
}
