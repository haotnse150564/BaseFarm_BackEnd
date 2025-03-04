using Application.Services;
using Microsoft.AspNetCore.Mvc;
using static Infrastructure.ViewModel.Request.AccountRequest;

namespace WebAPI.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountServices;

        public AccountController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var result = await _accountServices.LoginAsync(request.Email, request.Password);
            return Ok(result);
        }
    }
}
