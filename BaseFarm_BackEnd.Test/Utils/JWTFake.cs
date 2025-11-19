using Application.Utils;
using Application;
using Domain.Model;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace BaseFarm_BackEnd.Test.Utils
{
    public class JWTFake : JWTUtils
    {
        private readonly Account _user;

        public JWTFake(Account user = null!)
            : base(Mock.Of<IUnitOfWorks>(), Mock.Of<IConfiguration>(), Mock.Of<IHttpContextAccessor>())
        {
            _user = user;
        }

        public override Task<Account> GetCurrentUserAsync()
        {
            return Task.FromResult(_user);
        }

        public override string GenerateToken(Account account)
        {
            // Trả token giả để test LoginAsync không bị null exception
            return "fake-jwt-token";
        }
    }
}
