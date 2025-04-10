using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/farm-activity")]
    [ApiController]
    public class FarmActivityController : ControllerBase
    {
        private readonly IFarmActivityServices _farmActivityServices;
        public FarmActivityController(IFarmActivityServices farmActivityServices)
        {
            _farmActivityServices = farmActivityServices;
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetFarmActivities()
        {
            var result = await _farmActivityServices.GetFarmActivitiesAsync();
            return Ok(result);
        }
    }
    
    
}
