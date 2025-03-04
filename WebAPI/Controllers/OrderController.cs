using Application;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.ViewModel.Request.OrderRequest;
using static Application.ViewModel.Response.OrderResponse;

namespace WebAPI.Controllers
{
    [Route("api/v1/Order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderServices _orderService;

        public OrderController(IOrderServices orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO request)
        {
            if (request == null || request.OrderItems.Count == 0)
            {
                return BadRequest(new ResponseDTO(Const.FAIL_CREATE_CODE, "Order is Empty!!!."));
            }

            var result = await _orderService.CreateOrderAsync(request);

            if (result.Status != Const.SUCCESS_CREATE_CODE)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("orderList")]
        public async Task<IActionResult> GetListOrders([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _orderService.GetAllOrderAsync(pageIndex, pageSize);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }

        [HttpGet("orderListByCustomerId/{id}")]
        public async Task<IActionResult> GetListOrdersByCustomerId([FromRoute] long id, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _orderService.GetAllOrderByCustomerIdAsync(id, pageIndex, pageSize);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
    }
}
