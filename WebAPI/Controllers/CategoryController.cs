using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryServices _categoryServices;
        public CategoryController(ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryServices.GetAllCategoriesAsync();
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(long id)
        {
            var category = await _categoryServices.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound("Category not found.");
            }
            return Ok(category);
        }
        [HttpPost("create")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> CreateCategory([FromBody] string categoryName)
        {
            var category = await _categoryServices.CreateCategoryAsync(categoryName);
            if (category == null)
            {
                return BadRequest("Category creation failed.");
            }
            return Ok(category);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> UpdateCategory(long id, [FromBody] string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return BadRequest("Category name is required.");
            }
            var category = await _categoryServices.UpdateCategoryAsync(id, categoryName);
            if (category == null)
            {
                return NotFound("Category not found.");
            }
            return Ok(category);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> DeleteCategory(long id)
        {
            var result = await _categoryServices.DeleteCategoryAsync(id);
            if (result)
            {
                return NotFound("Category not found.");
            }
            return Ok("Deleted");
        }
    }
    
}
