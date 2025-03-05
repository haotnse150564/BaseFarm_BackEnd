using Application.Services;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            var response = await _accountServices.RegisterAsync(request);

            if (response.Status == 201)
                return Ok(response);

            return BadRequest(response);
        }
    }
}
