using Domain.Enums;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Utils
{
    public static class GenerateJWTString
    {
        public static string GenerateJsonWebToken(this Users user, string key, DateTime now, IConfiguration configuration )
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("Id", user.Id.ToString()),    
                new Claim("Role",Enum.GetName(typeof(Role),user.Role)),
                new Claim("Email", user.Email.ToString()),
                //new Claim("UserPermission", user.UserPermission != null ? user.UserPermission.ToString()! : "AccessDenied"),
                //new Claim("ImportPermission", user.ImportPermission != null ? user.ImportPermission.ToString()! : "AccessDenied"),
                //new Claim("InventoryPermission", user.InventoryPermission != null ? user.InventoryPermission.ToString()! : "AccessDenied"),
                //new Claim("InvoicePermission", user.InvoicePermission != null ? user.InvoicePermission.ToString()! : "AccessDenied"),
                //new Claim("OverTimeWorkSheetPermission", user.OverTimeWorkSheetPermission != null ? user.OverTimeWorkSheetPermission.ToString()! : "AccessDenied"),
                //new Claim("PushnimentPermisstion", user.PushnimentPermisstion != null ? user.PushnimentPermisstion.ToString()! : "AccessDenied"),
                //new Claim("ProductPermission", user.ProductPermission != null ? user.ProductPermission.ToString()! : "AccessDenied"),
                //new Claim("RequestPermission", user.RequestPermission != null ? user.RequestPermission.ToString()! : "AccessDenied"),
                //new Claim("VoucherPermission", user.VoucherPermission != null ? user.VoucherPermission.ToString()! : "AccessDenied"),
            };
            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims: claims, //sửa claims : claims
                expires: now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static string GenerateJsonWebTokenCustomExpireMinute(this Users user, string key, DateTime startTime, int minutes, IConfiguration configuration)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("CreatedDate", startTime.ToString()),
            };
            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims: claims,
                expires: startTime.AddMinutes(minutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}