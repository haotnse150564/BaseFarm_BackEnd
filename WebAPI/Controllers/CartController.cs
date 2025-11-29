using Application;
using Application.Services;
using Application.Services.Implement;
using Microsoft.AspNetCore.Mvc;
using static Application.ViewModel.Response.OrderResponse;

namespace WebAPI.Controllers
{
    [Route("api/v1/account")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartServices cartServices;
        public CartController(ICartServices cartServices)
        {
            this.cartServices = cartServices;
        }
        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart(long productId, int quantity)
        {
            var result = await cartServices.AddToCart(productId, quantity);
            if (!result)
                return BadRequest("Failed to add to cart");
            return Ok("Product added to cart successfully");
        }
        [HttpGet("cart-items")]
        public async Task<IActionResult> GetCartItems()
        {
            var cartItems = await cartServices.GetCartItems();
            return Ok(cartItems);
        }
        [HttpDelete("remove-cart-item")]
        public async Task<IActionResult> RemoveCartItem(long productId)
        {
            var result = await cartServices.RemoveCartItem(productId);
            if (result == null)
                return NotFound("Cart item not found");
            return Ok(result);
        }
        [HttpPut("update-cart-item")]
        public async Task<IActionResult> UpdateCartItem(int productId, int quantity)
        {
            var result = await cartServices.UpdateCartItem(productId, quantity);
            if (result == null)
                return NotFound("Cart item not found");
            return Ok(result);
        }
        [HttpDelete("clear-cart")]
        public async Task<IActionResult> ClearCart()
        {
            var result = await cartServices.ClearCart();
            if (result == null)
                return NotFound("Cart not found");
            return Ok(result);
        }

        [HttpPost("prepare-order")]
        public async Task<IActionResult> PrepareOrderAsync()
        {
            var createOrderDTO = await cartServices.PrepareOrderAsync(HttpContext);
            if (createOrderDTO == null)
            {
                return BadRequest(new ResponseDTO(400, "Cart is empty or unauthorized user."));
            }            

            return Ok(createOrderDTO.Data);
        }

        [HttpPost("buy-again/{orderId}")]
        public async Task<IActionResult> BuyAgain(long orderId)
        {
            var response = await cartServices.BuyAgainAsync(orderId);

            if (response.Status == Const.SUCCESS_CREATE_CODE)
                return Ok(response);

            if (response.Status == Const.FAIL_READ_CODE ||
                response.Status == Const.WARNING_NO_DATA_CODE)
                return BadRequest(response);

            if (response.Status == Const.FAIL_CREATE_CODE)
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}
