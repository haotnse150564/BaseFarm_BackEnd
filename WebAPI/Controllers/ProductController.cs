using Application;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.ViewModel.Request.ProductRequest;
using static Application.ViewModel.Response.ProductResponse;

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

        [HttpGet("products-list")]
        public async Task<IActionResult> GetListProducts([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _productService.GetAllProductAsync(pageIndex, pageSize);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }

        [HttpGet("get-product/{productId}")]
        public async Task<IActionResult> GetProductById([FromRoute] long productId)
        {
            
            var response = await _productService.GetProductByIdAsync(productId);

            // Trả về phản hồi
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi nếu không thành công
            }

            return Ok(response); // Trả về mã 200 nếu thành công
        }

        [HttpGet("search-product/{productName}")]
        public async Task<IActionResult> GetProductByName([FromRoute] string productName, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _productService.GetProductByNameAsync(productName, pageIndex, pageSize);

            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        //[Authorize(Roles = "Manager")]
        [HttpPost("create")]
        [Authorize(Roles = "Staff, Manager")]
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

        //[Authorize(Roles = "Admin")]
        [HttpPost("update/{id}")]
        [Authorize(Roles = "Staff, Manager")]
        public async Task<IActionResult> UpdateProductAsync([FromBody] CreateProductDTO request, [FromRoute] long id)
        {

            // Kiểm tra xem request có hợp lệ không
            if (request == null)
            {
                return BadRequest(new ResponseDTO(Const.FAIL_READ_CODE, "Invalid request."));
            }

            var response = await _productService.UpdateProductById(id, request);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }

        //[Authorize(Roles = "Admin, Manager")]
        [HttpPost("change-product-status/{id}")]
        [Authorize(Roles = "Staff, Manager")]

        public async Task<IActionResult> ChangeProductStatus([FromRoute] long id)
        {

            var response = await _productService.ChangeProductStatusById(id);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }

        [HttpPost("change-product-Quantity/{id}")]
        [Authorize(Roles = "Staff, Manager")]
        public async Task<IActionResult> ChangeProductQuantity([FromRoute] long id, [FromBody] UpdateQuantityDTO request)
        {

            var response = await _productService.ChangeProductQuantityById(id, request);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }
    }
}
