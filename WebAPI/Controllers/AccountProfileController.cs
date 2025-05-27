using Application.Services;
using Infrastructure.ViewModel.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/v1/account-profile")]
    [ApiController]
    public class AccountProfileController : ControllerBase
    {
        private readonly IAccountProfileServices _accountProfileServices;

        public AccountProfileController(IAccountProfileServices accountProfileServices)
        {
            _accountProfileServices = accountProfileServices;
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _accountProfileServices.ViewProfileAsync();
            return Ok(profile);
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] AccountProfileRequest.ProfileRequestDTO request)
        {
            var result = await _accountProfileServices.UpdateProfileAsync(request);
            if (result == null)
                return NotFound("Profile not found");

            return Ok(result);
        }
    }
}

