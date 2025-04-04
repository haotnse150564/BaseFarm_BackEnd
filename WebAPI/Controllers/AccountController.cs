using Application.Services;
using Infrastructure.ViewModel.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;
using static Infrastructure.ViewModel.Request.AccountRequest;

namespace WebAPI.Controllers
{
    [Route("api/v1/account")]
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
        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountForm request)
        {
            var result = await _accountServices.CreateAccountAsync(request);
            if (result != null)
                return Ok(result);
            return BadRequest("Failed to create account.");
        }
        [HttpPut("update-status/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAccount(long id)
        {
            var result = await _accountServices.UpdateAccountStatusAsync(id);
            if (result != null)
                return Ok(result);
            return BadRequest("Failed to update account.");
        }
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAccount(long id, [FromBody] AccountForm request)
        {
            var result = await _accountServices.UpdateAccountAsync(id, request);
            if (result != null)
                return Ok(result);
            return BadRequest("Failed to update account.");
        }
        [HttpGet("get-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAccount(int pageSize = 10, int pageIndex = 1)
        {
            var result = await _accountServices.GetAllAccountAsync(pageSize, pageIndex);
            if (result != null)
                return Ok(result);
            return BadRequest("Failed to get accounts.");
        }
    }
}
