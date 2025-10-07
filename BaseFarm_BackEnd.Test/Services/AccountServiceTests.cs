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
using static Infrastructure.ViewModel.Request.AccountRequest;

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

            // Setup UnitOfWork trả về các repository mock
            _mockUow.SetupGet(u => u.accountRepository)
                    .Returns(_mockAccountRepo.Object);

            _mockUow.SetupGet(u => u.accountProfileRepository)
                    .Returns(_mockAccountProfileRepo.Object);

            // Mock config JWTUtils
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Jwt:Key"]).Returns("12345678901234567890123456789012");
            mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("test_issuer");
            mockConfig.Setup(c => c["Jwt:Audience"]).Returns("test_audience");

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var realJwt = new JWTUtils(_mockUow.Object, mockConfig.Object, mockHttpContextAccessor.Object);

            _service = new AccountServices(
                _mockUow.Object,
                _mockMapper.Object,
                realJwt,
                _mockAccountProfileRepo.Object
            );
        }

        //login test
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

        //register test
        [Fact]
        public async Task RegisterAsync_EmailAlreadyExists_Returns400()
        {
            // Arrange
            var request = new RegisterRequestDTO
            {
                Email = "existing@mail.com",
                Password = "123456"
            };

            var existingAccount = new Account { Email = request.Email };
            _mockAccountRepo.Setup(r => r.GetByEmailAsync(request.Email))
                            .ReturnsAsync(existingAccount);

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.Equal(400, result.Status);
            Assert.Equal("Email already exists.", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_ValidRequest_Returns201()
        {
            // Arrange
            var request = new RegisterRequestDTO
            {
                Email = "newuser@mail.com",
                Password = "123456"
            };

            _mockAccountRepo.Setup(r => r.GetByEmailAsync(request.Email))
                            .ReturnsAsync((Account?)null);

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.Equal(201, result.Status);
            Assert.Equal("Registration successful.", result.Message);
            _mockAccountRepo.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once);
            _mockAccountProfileRepo.Verify(r => r.AddAsync(It.IsAny<AccountProfile>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_PasswordTooShort_ReturnsValidationError()
        {
            // Arrange
            var request = new RegisterRequestDTO
            {
                Email = "short@mail.com",
                Password = "123", // too short (< 6)
                ConfirmPassword = "123"
            };

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.Equal(400, result.Status);
            Assert.Equal("Password must be at least 6 characters.", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_ConfirmPasswordMismatch_ReturnsValidationError()
        {
            // Arrange
            var request = new RegisterRequestDTO
            {
                Email = "mismatch@mail.com",
                Password = "123456",
                ConfirmPassword = "654321" // mismatch
            };

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.Equal(400, result.Status);
            Assert.Equal("Passwords do not match.", result.Message);
        }
    }
}
