using Application.Services;
using Infrastructure.ViewModel.Request;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/config")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigService _service;

        public ConfigController(IConfigService service)
        {
            _service = service;
        }

        // GET: lấy config hiện tại
        [HttpGet]
        public ActionResult<AutoConfig> Get()
        {
            return Ok(_service.Get());
        }

        // POST: cập nhật config mới
        [HttpPost]
        public ActionResult<AutoConfig> Update([FromBody] AutoConfig config)
        {
            var updated = _service.Update(config);
            return Ok(updated);
        }
    }
}
