using Application;
using Application.Services;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpPost("inventory-create")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateInventory(long scheduleId, [FromBody] string location, int quantity, long productID)
        {
            var result = await _inventoryService.CalculateAndCreateInventoryAsync(quantity, location, productID, scheduleId);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
    }
}
