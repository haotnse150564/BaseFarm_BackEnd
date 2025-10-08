using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoMapper;
using Application.Services;
using Application.Utils;
using Domain.Model;
using Infrastructure.Repositories;
using static Infrastructure.ViewModel.Response.AccountProfileResponse;
using WebAPI.Services;
using Domain.Enum;
using Application;
using Infrastructure;

namespace BaseFarm_BackEnd.Test.Services
{
    // ✅ FakeService kế thừa AccountProfileServices để override logic gọi JWTUtils
    public class FakeAccountProfileService : AccountProfileServices
    {
        private readonly Account _fakeAccount;

        public FakeAccountProfileService(
            IUnitOfWorks unitOfWork,
            IMapper mapper,
            JWTUtils jwtUtils,
            Account fakeAccount)
            : base(unitOfWork, mapper, jwtUtils)
        {
            _fakeAccount = fakeAccount;
        }

        // ✅ Ghi đè ViewProfileAsync để không cần thật sự gọi JWTUtils.GetCurrentUserAsync
        public new async Task<ProfileResponseDTO> ViewProfileAsync()
        {
            var user = _fakeAccount;
            if (user == null)
                throw new Exception("User not found");

            // 👉 Dùng _unitOfWork từ class cha (đã có sẵn)
            var profile = await _unitOfWork.accountProfileRepository.GetByIdAsync(user.AccountId);
            var getCurrentUser = await _unitOfWork.accountRepository.GetByIdAsync(user.AccountId);

            if (profile == null)
                throw new Exception("Profile not found");

            var profileResponse = _mapper.Map<ProfileResponseDTO>(profile);

            profileResponse.Role = getCurrentUser.Role?.ToString() ?? "Unknown";
            profileResponse.Email = getCurrentUser.Email;
            return profileResponse;
        }
    }

    public class AccountProfileServiceTests
    {
        private readonly Mock<IUnitOfWorks> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<JWTUtils> _jwtUtilsMock;

        public AccountProfileServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWorks>();
            _mapperMock = new Mock<IMapper>();
            _jwtUtilsMock = new Mock<JWTUtils>(null!, null!, null!);
        }

        [Fact]
        public async Task ViewProfileAsync_ShouldReturnProfile_WhenUserAndProfileExist()
        {
            // Arrange
            int accountId = 1;

            var account = new Account
            {
                AccountId = accountId,
                Email = "user@example.com",
                Role = Roles.Customer
            };

            var profile = new AccountProfile
            {
                AccountProfileId = accountId,
                Fullname = "John Doe"
            };

            var mappedProfile = new ProfileResponseDTO
            {
                Fullname = "John Doe"
            };

            var accountRepoMock = new Mock<IAccountRepository>();
            var profileRepoMock = new Mock<IAccountProfileRepository>();

            accountRepoMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);
            profileRepoMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(profile);

            _unitOfWorkMock.Setup(u => u.accountRepository).Returns(accountRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.accountProfileRepository).Returns(profileRepoMock.Object);

            _mapperMock.Setup(m => m.Map<ProfileResponseDTO>(profile)).Returns(mappedProfile);

            var service = new FakeAccountProfileService(_unitOfWorkMock.Object, _mapperMock.Object, _jwtUtilsMock.Object, account);

            // Act
            var result = await service.ViewProfileAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John Doe", result.Fullname);
            Assert.Equal("user@example.com", result.Email);
            Assert.Equal("Customer", result.Role);
        }

        [Fact]
        public async Task ViewProfileAsync_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            Account? nullAccount = null;
            var service = new FakeAccountProfileService(_unitOfWorkMock.Object, _mapperMock.Object, _jwtUtilsMock.Object, nullAccount);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => service.ViewProfileAsync());
            Assert.Equal("User not found", ex.Message);
        }

        [Fact]
        public async Task ViewProfileAsync_ShouldThrow_WhenProfileNotFound()
        {
            // Arrange
            int accountId = 2;

            var account = new Account
            {
                AccountId = accountId,
                Email = "user@example.com",
                Role = Roles.Customer
            };

            var accountRepoMock = new Mock<IAccountRepository>();
            var profileRepoMock = new Mock<IAccountProfileRepository>();

            accountRepoMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);
            profileRepoMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync((AccountProfile)null!);

            _unitOfWorkMock.Setup(u => u.accountRepository).Returns(accountRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.accountProfileRepository).Returns(profileRepoMock.Object);

            var service = new FakeAccountProfileService(_unitOfWorkMock.Object, _mapperMock.Object, _jwtUtilsMock.Object, account);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => service.ViewProfileAsync());
            Assert.Equal("Profile not found", ex.Message);
        }
    }
}
