using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/v1/account")]
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
    }
}
