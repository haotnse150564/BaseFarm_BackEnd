using Xunit;
using Moq;
using System;
using Application.Services;
using System.Threading.Tasks;
using System.Security.Principal;
using Application.Utils;
using Domain.Model;
using Infrastructure.Repositories;
using Domain.Enum;
using Application;
using AutoMapper;
using WebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BaseFarm_BackEnd.Test.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<IUnitOfWorks> _mockUow;
        private readonly Mock<IAccountRepository> _mockAccountRepo;
        private readonly Mock<IAccountProfileRepository> _mockAccountProfileRepo;
        private readonly Mock<JWTUtils> _mockJwt;
        private readonly AccountServices _service;
        private readonly Mock<IMapper> _mockMapper;

        public AccountServiceTests()
        {
            _mockUow = new Mock<IUnitOfWorks>();
            _mockAccountRepo = new Mock<IAccountRepository>();
            _mockAccountProfileRepo = new Mock<IAccountProfileRepository>();
            _mockMapper = new Mock<IMapper>();

            // setup unit of work trả về accountRepository
            _mockUow.SetupGet(u => u.accountRepository).Returns(_mockAccountRepo.Object);

            // 🧩 Mock config cho JWTUtils
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Jwt:Key"]).Returns("12345678901234567890123456789012");
            mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("test_issuer");
            mockConfig.Setup(c => c["Jwt:Audience"]).Returns("test_audience");

            // 🧩 Mock HttpContextAccessor (có thể để rỗng)
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            // 🧩 Tạo JWTUtils thật với mock dependencies
            var realJwt = new JWTUtils(_mockUow.Object, mockConfig.Object, mockHttpContextAccessor.Object);

            // ✅ Khởi tạo AccountServices
            _service = new AccountServices(
                _mockUow.Object,
                _mockMapper.Object,
                realJwt,
                _mockAccountProfileRepo.Object
            );
        }

        [Fact]
        public async Task LoginAsync_EmailNotFound_ThrowsUnauthorizedAccess()
        {
            // Arrange
            string email = "notfound@mail.com";
            _mockAccountRepo.Setup(r => r.GetByEmailAsync(email))
                            .ReturnsAsync((Account?)null);

            // Act + Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.LoginAsync(email, "123123"));

            Assert.Equal("Invalid email.", ex.Message);
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_ThrowsUnauthorizedAccess()
        {
            // Arrange
            var account = new Account
            {
                Email = "truong@mail.com",
                PasswordHash = PasswordHelper.HashPassword("123123"),
                Status = AccountStatus.ACTIVE
            };
            _mockAccountRepo.Setup(r => r.GetByEmailAsync(account.Email))
                            .ReturnsAsync(account);

            // Act + Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.LoginAsync(account.Email, "wrongpass"));

            Assert.Equal("Invalid password.", ex.Message);
        }

        [Fact]
        public async Task LoginAsync_AccountInactive_ThrowsUnauthorizedAccess()
        {
            // Arrange
            var account = new Account
            {
                Email = "inactive@mail.com",
                PasswordHash = PasswordHelper.HashPassword("123123"),
                Status = AccountStatus.BANNED
            };
            _mockAccountRepo.Setup(r => r.GetByEmailAsync(account.Email))
                            .ReturnsAsync(account);

            // Act + Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.LoginAsync(account.Email, "123123"));

            Assert.Equal("Account is not accitve.", ex.Message);
        }

        [Fact]
        public async Task LoginAsync_ValidAccount_ReturnsToken()
        {
            // Arrange
            var account = new Account
            {
                Email = "truong@mail.com",
                PasswordHash = PasswordHelper.HashPassword("123123"),
                Status = AccountStatus.ACTIVE
            };
            _mockAccountRepo.Setup(r => r.GetByEmailAsync(account.Email))
                            .ReturnsAsync(account);

            // Act
            var result = await _service.LoginAsync(account.Email, "123123");

            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.Token)); // kiểm tra token thật có sinh ra
        }

    }
}
