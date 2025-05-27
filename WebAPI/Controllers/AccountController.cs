using Application.Services;
using Domain.Enum;
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
        [Authorize(Roles = "1")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountForm request)
        {
            var result = await _accountServices.CreateAccountAsync(request);
            if (result != null)
                return Ok(result);
            return BadRequest("Failed to create account.");
        }
        [HttpPut("update-status/{id}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateAccount(long id)
        {
            var result = await _accountServices.UpdateAccountStatusAsync(id);
            if (result != null)
                return Ok(result);
            return BadRequest("Failed to update account.");
        }
        [HttpPut("update/{id}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateAccount(long id, [FromBody] AccountForm request)
        {
            var result = await _accountServices.UpdateAccountAsync(id, request);
            if (result != null)
                return Ok(result);
            return BadRequest("Failed to update account.");
        }

        [HttpGet("get-all")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetAllAccount(int pageSize = 10, int pageIndex = 1, AccountStatus? status = null, Roles? role = null)
        {
            try
            {
                // Gọi dịch vụ để lấy danh sách tài khoản với bộ lọc status và role
                var result = await _accountServices.GetAllAccountAsync(pageSize, pageIndex, status, role);

                if (result != null)
                {
                    return Ok(result); // Trả về kết quả nếu tìm thấy
                }

                // Trả về BadRequest nếu không có dữ liệu
                return BadRequest("Failed to get accounts.");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get-by-email")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetAllEmailAccount(string email)
        {
            var result = await _accountServices.GetAccountByEmail(email);
            if (result != null)
                return Ok(result);
            return BadRequest("Failed to get accounts.");
        }
        [HttpPut("update-role")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateAccountRole(long accountId, int roleId)
        {
            var result = await _accountServices.UpdateRoleForUser(accountId, roleId);
            if (result != null)
                return Ok(result);
            return BadRequest("Failed to update roles.");
        }
        [HttpPut("update-password")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> ChangPassword(long id, [FromBody] ChangePasswordDTO request)
        {
            var result = await _accountServices.ChangePassword(id, request);
            if (result != null)
                return Ok(result);
            return BadRequest("Failed to update roles.");
        }
    }
}
