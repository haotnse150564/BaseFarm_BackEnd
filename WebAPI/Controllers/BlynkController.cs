using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/blynk")]
    public class BlynkController : ControllerBase
    {
        private readonly IBlynkService _blynkService;

        public BlynkController(IBlynkService blynkService)
        {
            _blynkService = blynkService;
        }

        [HttpGet("get-blynk-data")]
        public async Task<IActionResult> GetBlynkData()
        {
            var data = await _blynkService.GetAllDatastreamValuesAsync();
            return Ok(data);
        }
    }
}
