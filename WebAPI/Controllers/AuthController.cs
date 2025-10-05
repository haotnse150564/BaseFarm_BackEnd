using Application;
using Application.Utils;
using Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JWTUtils _jwtUtils;
        private readonly IUnitOfWorks _UnitOfWorks;
        public AuthController(JWTUtils jwtUtils, UnitOfWorks unitOfWorks)
        {
            _jwtUtils = jwtUtils;
            _UnitOfWorks = unitOfWorks;
        }
        // BƯỚC 1: Bắt đầu quy trình OAuth (Chuyển hướng đến Google)
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            // 1. Đặt URI để Google trả về sau khi xác thực thành công
            var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", null, Request.Scheme);

            // 2. Trả về ChallengeResult để ASP.NET Core tự động chuyển hướng đến Google
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            // Sử dụng scheme "Google" để kích hoạt provider Google
            return Challenge(properties, "Google");
        }

        // BƯỚC 2: Xử lý phản hồi từ Google (Callback)
        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleCallback()
        {
            // Lấy thông tin xác thực từ cookie tạm thời ("External")
            var authenticateResult = await HttpContext.AuthenticateAsync("External");

            if (!authenticateResult.Succeeded)
            {
                // Xử lý lỗi nếu xác thực thất bại
                return BadRequest(new { Error = "Google authentication failed." });
            }

            // --- LẤY DỮ LIỆU NGƯỜI DÙNG ---
            var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
            var name = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);
            var issuer = authenticateResult.Principal.FindFirstValue("iss");

            if(email == null)
            {
                return BadRequest(new { Error = "Email chưa được đăng ký." });
            }

            var account = await _UnitOfWorks.accountRepository.GetByEmailAsync(email);
            var jwtToken = _jwtUtils.GenerateToken(account); // Tự implement logic tạo JWT

            // Xóa cookie tạm thời
            await HttpContext.SignOutAsync("External");

            // Trả về token cho client
            return Ok(new { Token = jwtToken, Email = email, Name = name });
        }
    }
}
