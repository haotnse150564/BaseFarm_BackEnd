using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
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

            // **LƯU Ý QUAN TRỌNG:**
            // 1. KIỂM TRA: Tìm người dùng trong cơ sở dữ liệu bằng 'email' (hoặc 'issuer' + 'id')
            // 2. TẠO: Nếu chưa tồn tại, tạo tài khoản người dùng mới.

            // --- TẠO TOKEN JWT (Thường dùng cho API) ---
            // Thay vì chỉ trả về email, bạn cần tạo và trả về một JWT (JSON Web Token)
            // để client (ứng dụng di động/web) sử dụng cho các yêu cầu API tiếp theo.

            var jwtToken = "YOUR_GENERATED_JWT_TOKEN"; // Tự implement logic tạo JWT

            // Xóa cookie tạm thời
            await HttpContext.SignOutAsync("External");

            // Trả về token cho client
            return Ok(new { Token = jwtToken, Email = email, Name = name });
        }
    }
