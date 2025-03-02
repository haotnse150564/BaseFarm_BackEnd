using Application;
using Application.Services;
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
    }
}
