using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/crop")]
    [ApiController]
    public class CropController : ControllerBase
    {
        private readonly ICropServices _cropServices;
        public CropController(ICropServices cropServices)
        {
            _cropServices = cropServices;
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCrops()
        {
            var result = await _cropServices.GetAllCropsAsync();
            return Ok(result);
        }
    }
}
