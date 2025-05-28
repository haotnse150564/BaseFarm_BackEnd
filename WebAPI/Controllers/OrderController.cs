using Application;
using Application.Services;
using Application.ViewModel.Request;
using Domain.Enum;
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

        [HttpGet("order-list")]
        [Authorize]
        public async Task<IActionResult> GetListOrders([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] PaymentStatus? status = null)
        {
            var result = await _orderService.GetAllOrderAsync(pageIndex, pageSize, status);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách đơn hàng với phân trang và lọc theo status nếu có
        }


        [HttpGet("order-list-by-customer/{id}")]
        [Authorize]
        public async Task<IActionResult> GetListOrdersByCustomerId([FromRoute] long id, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] PaymentStatus? status = null)
        {
            var result = await _orderService.GetAllOrderByCustomerIdAsync(id, pageIndex, pageSize, status);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }

        [HttpGet("order-list-by-current-account")]
        [Authorize]
        public async Task<IActionResult> GetListOrdersByCurrentCustomer([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] PaymentStatus? status = null)
        {
            var result = await _orderService.GetAllOrderByCurrentCustomerAsync(pageIndex, pageSize, status);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("order/{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(long orderId)
        {
            var result = await _orderService.GetOrderByIdAsync(orderId);

            if (result.Status == Const.FAIL_READ_CODE)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("order-list-by-customer-name/{name}")]
        [Authorize]
        public async Task<IActionResult> GetListOrdersByCustomerName([FromRoute] string name, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _orderService.GetAllOrderByCustomerNameAsync(name, pageIndex, pageSize);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách đơn hàng với phân trang
        }
        [HttpGet("order-list-by-emal/{email}")]
        [Authorize]
        public async Task<IActionResult> GetListOrderbyEmail([FromRoute] string email, [FromQuery] PaymentStatus? status , [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _orderService.SearchOrderbyEmail(email, pageIndex, pageSize, status);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách đơn hàng với phân trang
        }
        [HttpPost("order-list-by-date")]
        [Authorize]
        public async Task<IActionResult> GetListOrdersByCustomerName([FromBody] DateOnly date, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _orderService.SearchOrderbyCreateDate(date, pageIndex, pageSize);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách đơn hàng với phân trang
        }

        [HttpPut("updateDeliveryStatus/{orderId}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderDeliveryStatus(long orderId)
        {
            var result = await _orderService.UpdateOrderDeliveryStatusAsync(orderId);

            if (result.Status == Const.FAIL_READ_CODE)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut("updateCompletedStatus/{orderId}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderCompletedStatus(long orderId)
        {
            var result = await _orderService.UpdateOrderCompletedStatusAsync(orderId);

            if (result.Status == Const.FAIL_READ_CODE)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut("updateCancelStatus/{orderId}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderCancelledStatus(long orderId)
        {
            var result = await _orderService.UpdateOrderCancelStatusAsync(orderId);

            if (result.Status == Const.FAIL_READ_CODE)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("createOrderPayment/{orderId}")]
        public async Task<IActionResult> CreateOrderPayment(long orderId)
        {
            var result = await _orderService.CreateOrderPaymentAsync(orderId, HttpContext);
            return Ok(result);
        }

    }
}
