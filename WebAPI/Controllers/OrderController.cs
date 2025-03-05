using Application;
using Application.Services;
using Application.ViewModel.Request;
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
        private readonly IVnPayService _vnPayService;
        private readonly ILogger<OrderController> _logger;
        public OrderController(IOrderServices orderService, IVnPayService vnPayService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _vnPayService = vnPayService;
            _logger = logger;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO request)
        {
            if (request == null || !request.OrderItems.Any())
            {
                return BadRequest(new { message = "Invalid order request." });
            }

            try
            {
                // 🔥 Gọi CreateOrderAsync với HttpContext
                var orderResponse = await _orderService.CreateOrderAsync(request, HttpContext);

                if (orderResponse.Status != Const.SUCCESS_CREATE_CODE)
                {
                    return BadRequest(orderResponse);
                }

                return Ok(orderResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating order.");
                return StatusCode(500, new { message = "An error occurred while creating the order." });
            }
        }


        //[HttpPost("createOrder")]
        //public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO request)
        //{
        //    if (request == null || request.OrderItems.Count == 0)
        //    {
        //        return BadRequest(new ResponseDTO(Const.FAIL_CREATE_CODE, "Order is Empty!!!."));
        //    }

        //    var result = await _orderService.CreateOrderAsync(request);

        //    if (result.Status != Const.SUCCESS_CREATE_CODE)
        //    {
        //        return BadRequest(result);
        //    }

        //    return Ok(result);
        //}

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

        [HttpGet("orderById/{orderId}")]
        public async Task<IActionResult> GetOrderById(long orderId)
        {
            var result = await _orderService.GetOrderByIdAsync(orderId);

            if (result.Status == Const.FAIL_READ_CODE)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

    }
}
