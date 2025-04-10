using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/farm")]
    [ApiController]
    public class FarmDetailController : ControllerBase
    {
        private readonly IFarmServices _farmDetailServices;
        public FarmDetailController(IFarmServices farmDetailServices)
        {
            _farmDetailServices = farmDetailServices;
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllFarms()
        {
            var result = await _farmDetailServices.GetAll();
            return Ok(result);
        }
    }

}
