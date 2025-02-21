using Application.Services.Implement;
using System.Security.Claims;

namespace WebAPI.Services
{
    public class ClaimsServices : IClaimsServices
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public ClaimsServices(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext != null)
            {
                var id = httpContextAccessor.HttpContext.User.FindFirstValue("Id");
                GetCurrentUserId = id == null ? 8 : int.Parse(id);
            }
        }
        public int GetCurrentUserId { get; }
    }
}

