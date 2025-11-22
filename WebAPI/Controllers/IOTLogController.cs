using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/Log")]
    [ApiController]
    public class IOTLogController : ControllerBase
    {
        private readonly ILogger<IOTLogController> _logger;

        public IOTLogController(ILogger<IOTLogController> logger)
        {
            _logger = logger;
        }


    }
}
