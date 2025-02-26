using Application;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.ViewModel.Request.ProductRequest;

namespace WebAPI.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productService;

        public ProductController(IProductServices productService)
        {
            _productService = productService;
        }

        [HttpGet("productsList")]
        public async Task<IActionResult> GetListProducts([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _productService.GetAllProductAsync(pageIndex, pageSize);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }

        [HttpGet("GetProductById/{productId}")]
        public async Task<IActionResult> GetProductById([FromRoute] int productId)
        {
            
            var response = await _productService.GetProductByIdAsync(productId);

            // Trả về phản hồi
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi nếu không thành công
            }

            return Ok(response); // Trả về mã 200 nếu thành công
        }

        [HttpGet("searchProductByName/{productName}")]
        public async Task<IActionResult> GetProductByName([FromRoute] string productName)
        {
            
            var response = await _productService.GetProductByNameAsync(productName);

            // Trả về phản hồi
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi nếu không thành công
            }

            return Ok(response); // Trả về mã 200 nếu thành công
        }

        //[Authorize(Roles = "Manager")]
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO createRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _productService.CreateProductAsync(createRequest);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
