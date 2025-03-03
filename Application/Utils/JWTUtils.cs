using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Utils
{
    public class JWTUtils
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JWTUtils(IUnitOfWorks unitOfWork, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateToken(Account account)
        {
            var tokenSecret = _config["Jwt:Key"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(tokenSecret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Role, account.Role!.ToString()!.Trim())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Không cho phép chênh lệch thời gian
                };

                return tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                
            }
            catch (SecurityTokenExpiredException ex)
            {
                Console.WriteLine("Token expired: " + ex.Message);
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                Console.WriteLine("Invalid token signature: " + ex.Message);
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine("Invalid token: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error in ValidateToken: " + ex.Message);
            }
            return null;
        }

        public async Task<Account> GetCurrentUserAsync()
        {
            // Lấy token từ header Authorization
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                Account existUser = null;
                return existUser;
            }

            // Giải mã token để lấy thông tin người dùng
            var claimsPrincipal = ValidateToken(token);

            // Lấy UserId từ claims
            var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new Exception("User not found in token."); // Hoặc trả về một ResponseDTO nếu cần
            }

            long userId = Convert.ToInt64(userIdClaim.Value);

            // Lấy người dùng hiện tại
            var user = await _unitOfWork.accountRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found!"); // Hoặc trả về một ResponseDTO nếu cần
            }

            return user;
        }
        public bool IsExpiredToken(string token, DateTime now)
        {
            JwtSecurityToken jwt = new JwtSecurityToken(token);
            return jwt.ValidTo < now;
        }
    }
}
