using Application.Services;
using Application.Services.Implement;
using Infrastructure.ViewModel.Request;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/FarmEquipment")]
    public class FarmEquipmentController : ControllerBase
    {
        private readonly IFarmEquipmentServices _farmEquipmentServices;
        public FarmEquipmentController(IFarmEquipmentServices farmEquipmentServices)
        {
            _farmEquipmentServices = farmEquipmentServices;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _farmEquipmentServices.GetAll();
            return Ok(result);
        }
        [HttpPost("CreateFarmEquipment")]
        public async Task<IActionResult> CreateFarmEquipment([FromBody] Infrastructure.ViewModel.Request.FarmEquipmentRequest request)
        {
            var result = await _farmEquipmentServices.CreateFarmEquipment(request);
            return Ok(result);
        }
        [HttpGet("GetFarmEquipmentByDevicesName")]
        public async Task<IActionResult> GetFarmEquipmentByDevicesName([FromQuery] string name)
        {
            var result = await _farmEquipmentServices.GetFarmEquipmentByDevicesName(name);
            return Ok(result);
        }
        [HttpGet("GetListFarmEquipmentActive")]
        public async Task<IActionResult> GetListEquipmentActive()
        {
            var result = await _farmEquipmentServices.GetListEquipmentActive();
            return Ok(result);
        }
        [HttpPut("RemmoveFarmEquipment")]
        public async Task<IActionResult> RemmoveFarmEquipment([FromQuery] long id)
        {
            var result = await _farmEquipmentServices.RemmoveFarmEquipment(id);
            return Ok(result);
        }

        [HttpPut("update-farm-equipment")]
        public async Task<IActionResult> UpdateFarmEquipment(long farmEquipmentId, [FromBody] FarmEquipmentRequest request)
        {
            var result = await _farmEquipmentServices.UpdateFarmEquipment(farmEquipmentId, request);
            return Ok(result);
        }
    }
}
