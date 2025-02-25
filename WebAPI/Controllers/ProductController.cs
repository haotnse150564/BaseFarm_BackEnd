using Application;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetListProducts()
        {
            var result = await _productService.GetAllProductAsync();
            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }
            return Ok(result);
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
    }
}
