using Application;
using Application.Services;
using Infrastructure.ViewModel.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{

    [Route("api/v1/iotDevices")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IDevicesServices _device;
        private readonly ILogger<OrderController> _logger;

        public DevicesController(IDevicesServices Devices, ILogger<OrderController> logger)
        {
            _device = Devices;

            _logger = logger;
        }
        [HttpPost("iotDevices-create")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateiotDevices([FromBody] IOTRequest request)
        {
            var result = await _device.CreateDeviceAsync(request);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }

        [HttpGet("iotDevices-list")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetListiotDevices([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _device.GetAllDevices(pageIndex, pageSize);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
        [HttpGet("iotDevices-byId")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetiotDevicesByID(long id)
        {
            var result = await _device.GetDeviceById(id);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
        [HttpPut("iotDevices-update-status")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateiotDevicesStatus(long iotDevicesId, [FromBody] string status)
        {
            var result = await _device.UpdateStatusDeviceAsync(iotDevicesId, status.ToUpper());

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
        [HttpPatch("iotDevices-update")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateiotDevices(long iotDevicesId, [FromBody] IOTRequest request)
        {
            var result = await _device.UpdateDeviceAsync(iotDevicesId, request);

            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về lỗi 400 nếu thất bại
            }

            return Ok(result); // Trả về danh sách sản phẩm với phân trang
        }
    }
}
