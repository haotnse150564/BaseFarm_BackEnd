using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/Log")]
    [ApiController]
    public class IOTLogController : ControllerBase
    {
        private readonly IIOTLogServices iOTLogServices;
        public IOTLogController(IIOTLogServices iOTLogServices)
        {
            this.iOTLogServices = iOTLogServices;
        }

        [HttpGet("UpdateLog")]
        public ActionResult UpdateLog()
        {
            iOTLogServices.UpdateLogAsync();
            return Ok("Log update initiated.");
        }
        [HttpGet("GetLogs")]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await iOTLogServices.GetList();
            return Ok(logs);
        }
    }
}
