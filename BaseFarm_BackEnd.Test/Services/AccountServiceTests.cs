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
using static Infrastructure.ViewModel.Response.AccountResponse;

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
                Password = "123456",
                ConfirmPassword = "123456"
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
                Password = "123456",
                ConfirmPassword = "123456"
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

        //change password test
        [Fact]
        public async Task ChangePassword_AccountNotFound_Returns404()
        {
            // Arrange
            long id = 1;
            _mockAccountRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Account?)null);

            var request = new ChangePasswordDTO
            {
                OldPassword = "123123",
                NewPassword = "654321",
                ConfirmPassword = "654321"
            };

            // Act
            var result = await _service.ChangePassword(id, request);

            // Assert
            Assert.Equal(404, result.Status);
            Assert.Equal("Account not found", result.Message);
        }

        [Fact]
        public async Task ChangePassword_WrongOldPassword_Returns400()
        {
            // Arrange
            var account = new Account
            {
                AccountId = 1,
                PasswordHash = PasswordHelper.HashPassword("123123")
            };
            _mockAccountRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(account);

            var request = new ChangePasswordDTO
            {
                OldPassword = "321321",
                NewPassword = "654321",
                ConfirmPassword = "654321"
            };

            // Act
            var result = await _service.ChangePassword(1, request);

            // Assert
            Assert.Equal(400, result.Status);
            Assert.Equal("Old password is incorrect", result.Message);
        }

        [Fact]
        public async Task ChangePassword_PasswordMismatch_Returns400()
        {
            // Arrange
            var account = new Account
            {
                AccountId = 1,
                PasswordHash = PasswordHelper.HashPassword("123123")
            };
            _mockAccountRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(account);

            var request = new ChangePasswordDTO
            {
                OldPassword = "123123",
                NewPassword = "654321",
                ConfirmPassword = "321321"
            };

            // Act
            var result = await _service.ChangePassword(1, request);

            // Assert
            Assert.Equal(400, result.Status);
            Assert.Equal("New password and confirm password do not match", result.Message);
        }

        [Fact]
        public async Task ChangePassword_BlankPassword_Returns400()
        {
            // Arrange
            var account = new Account
            {
                AccountId = 1,
                PasswordHash = PasswordHelper.HashPassword("123123")
            };
            _mockAccountRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(account);

            var request = new ChangePasswordDTO
            {
                OldPassword = "123123",
                NewPassword = "   ",
                ConfirmPassword = "   "
            };

            // Act
            var result = await _service.ChangePassword(1, request);

            // Assert
            Assert.Equal(400, result.Status);
            Assert.Equal("Password do not have space or blank", result.Message);
        }

        [Fact]
        public async Task ChangePassword_ValidRequest_Returns200()
        {
            // Arrange
            var account = new Account
            {
                AccountId = 1,
                PasswordHash = PasswordHelper.HashPassword("123123")
            };
            _mockAccountRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(account);

            var request = new ChangePasswordDTO
            {
                OldPassword = "123123",
                NewPassword = "654321",
                ConfirmPassword = "654321"
            };

            // Act
            var result = await _service.ChangePassword(1, request);

            // Assert
            Assert.Equal(200, result.Status);
            Assert.Equal("Change password success", result.Message);

            _mockAccountRepo.Verify(r => r.UpdateAsync(It.IsAny<Account>()), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }


        //create account test
        [Fact]
        public async Task CreateAccountAsync_EmailExists_ThrowsException()
        {
            // Arrange
            var request = new AccountForm
            {
                Email = "test@mail.com",
                Gender = Gender.Male,
                Role = 1,
                Phone = "0123456789",
                Fullname = "Test User",
                Address = "Hanoi"
            };
            _mockAccountRepo.Setup(r => r.GetByEmailAsync(request.Email))
                            .ReturnsAsync(new Account());

            // Act + Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateAccountAsync(request));
            Assert.Equal("Email already exists!", ex.Message);
        }

        [Fact]
        public async Task CreateAccountAsync_SaveChangesFails_ThrowsException()
        {
            // Arrange
            var request = new AccountForm
            {
                Email = "new@mail.com",
                Gender = Gender.Female,
                Role = 1,
                Phone = "0987654321",
                Fullname = "New User",
                Address = "Danang"
            };

            _mockAccountRepo.Setup(r => r.GetByEmailAsync(request.Email))
                            .ReturnsAsync((Account?)null);
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(-1);

            // Act + Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateAccountAsync(request));
            Assert.Equal("Create Fail!", ex.Message);
        }

        [Fact]
        public async Task CreateAccountAsync_ValidRequest_ReturnsMappedViewAccount()
        {
            // Arrange
            var request = new AccountForm
            {
                Email = "valid@mail.com",
                Gender = Gender.Male,
                Role = 1,
                Phone = "0999888777",
                Fullname = "Valid User",
                Address = "Hue"
            };

            _mockAccountRepo.Setup(r => r.GetByEmailAsync(request.Email))
                            .ReturnsAsync((Account?)null);
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var createdAccount = new Account { Email = request.Email };
            _mockMapper.Setup(m => m.Map<ViewAccount>(It.IsAny<Account>()))
                       .Returns(new ViewAccount { Email = request.Email });

            // Act
            var result = await _service.CreateAccountAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Email, result.Email);
        }

        [Fact]
        public async Task CreateAccountAsync_MissingEmail_ThrowsException()
        {
            // Arrange
            var request = new AccountForm
            {
                Email = null!, // missing required field
                Gender = Gender.Male,
                Role = 1,
                Phone = "0112233445",
                Fullname = "No Email User",
                Address = "Hanoi"
            };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.CreateAccountAsync(request));
        }

        //update account test
        [Fact]
        public async Task UpdateAccountAsync_AccountNotFound_ThrowsException()
        {
            // Arrange
            long id = 1;
            _mockAccountRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Account?)null);

            var form = new AccountForm { Email = "test@mail.com" };

            // Act + Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.UpdateAccountAsync(id, form));

            Assert.Equal("Acoount not found", ex.Message);
        }

        [Fact]
        public async Task UpdateAccountAsync_EmailAlreadyUsed_ThrowsException()
        {
            // Arrange
            long id = 1;
            var account = new Account
            {
                AccountId = id,
                Email = "old@mail.com",
                AccountProfile = new AccountProfile { AccountProfileId = 1 }
            };

            _mockAccountRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(account);
            _mockAccountRepo.Setup(r => r.GetAllAsync())
                            .ReturnsAsync(new List<Account> {
                        new Account { Email = "used@mail.com" }
                            });

            var form = new AccountForm
            {
                Email = "used@mail.com", // email bị trùng
                Phone = "0123456789",
                Fullname = "Test",
                Gender = Gender.Male,
                Address = "Hanoi"
            };

            // Act + Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.UpdateAccountAsync(id, form));

            Assert.Equal("Email has been used", ex.Message);
        }

        [Fact]
        public async Task UpdateAccountAsync_SaveFails_ThrowsException()
        {
            // Arrange
            long id = 1;
            var account = new Account
            {
                AccountId = id,
                Email = "used@mail.com",
                AccountProfile = new AccountProfile { AccountProfileId = 1 }
            };

            _mockAccountRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(account);
            _mockAccountRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Account>());
            _mockAccountProfileRepo.Setup(r => r.GetByIdAsync(1))
                                   .ReturnsAsync(new AccountProfile());

            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(-1);

            var form = new AccountForm
            {
                Email = "new@mail.com",
                Phone = "0123456789",
                Fullname = "Test",
                Gender = Gender.Male,
                Address = "Hanoi"
            };

            // Act + Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.UpdateAccountAsync(id, form));

            Assert.Equal("Update Fail!", ex.Message);
        }

        [Fact]
        public async Task UpdateAccountAsync_ValidRequest_ReturnsUpdatedAccount()
        {
            // Arrange
            long id = 1;
            var account = new Account
            {
                AccountId = id,
                Email = "old@mail.com",
                Role = Roles.Customer,
                Status = AccountStatus.ACTIVE,
                AccountProfile = new AccountProfile { AccountProfileId = 1 }
            };

            _mockAccountRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(account);
            _mockAccountRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Account>());
            _mockAccountProfileRepo.Setup(r => r.GetByIdAsync(1))
                                   .ReturnsAsync(new AccountProfile { AccountProfileId = 1 });

            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _mockMapper.Setup(m => m.Map<ViewAccount>(It.IsAny<Account>()))
                       .Returns((Account acc) => new ViewAccount
                       {
                           AccountId = acc.AccountId,
                           Email = acc.Email
                       });

            var form = new AccountForm
            {
                Email = "new@mail.com",
                Phone = "0123456789",
                Fullname = "Updated User",
                Gender = Gender.Male,
                Address = "Hanoi"
            };

            // Act
            var result = await _service.UpdateAccountAsync(id, form);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("new@mail.com", result.Email);
        }

        //ban account test
        [Fact]
        public async Task UpdateAccountStatusAsync_ActiveAccount_BecomesBanned()
        {
            // Arrange
            long accountId = 1;
            var account = new Account
            {
                AccountId = accountId,
                Status = AccountStatus.ACTIVE
            };

            _mockUow.Setup(u => u.accountRepository.GetByIdAsync(accountId))
                    .ReturnsAsync(account);

            _mockMapper.Setup(m => m.Map<ViewAccount>(It.IsAny<Account>()))
                       .Returns(new ViewAccount { AccountId = accountId, Status = "BANNED" });

            // Act
            var result = await _service.UpdateAccountStatusAsync(accountId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("BANNED", result.Status);
            _mockUow.Verify(u => u.accountRepository.UpdateAsync(It.Is<Account>(a => a.Status == AccountStatus.BANNED)), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAccountStatusAsync_AccountNotFound_ReturnsNull()
        {
            // Arrange
            long accountId = 99;
            _mockUow.Setup(u => u.accountRepository.GetByIdAsync(accountId))
                    .ReturnsAsync((Account?)null);

            // Act
            var result = await _service.UpdateAccountStatusAsync(accountId);

            // Assert
            Assert.Null(result);
            _mockUow.Verify(u => u.accountRepository.UpdateAsync(It.IsAny<Account>()), Times.Never);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAccountStatusAsync_BannedAccount_BecomesActive()
        {
            // Arrange
            long accountId = 2;
            var account = new Account
            {
                AccountId = accountId,
                Status = AccountStatus.BANNED
            };

            _mockUow.Setup(u => u.accountRepository.GetByIdAsync(accountId))
                    .ReturnsAsync(account);

            _mockMapper.Setup(m => m.Map<ViewAccount>(It.IsAny<Account>()))
                       .Returns(new ViewAccount { AccountId = accountId, Status = "ACTIVE" });

            // Act
            var result = await _service.UpdateAccountStatusAsync(accountId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ACTIVE", result.Status);
            _mockUow.Verify(u => u.accountRepository.UpdateAsync(It.Is<Account>(a => a.Status == AccountStatus.ACTIVE)), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        //get all account filter test
        [Fact]
        public async Task GetAllAccountAsync_WhenRepositoryReturnsEmptyList_ReturnsNull()
        {
            // Arrange
            _mockAccountRepo.Setup(r => r.GetAllAccountWithProfiles(null, null))
                            .ReturnsAsync(new List<Account>());

            // Act
            var result = await _service.GetAllAccountAsync(10, 1, null, null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAccountAsync_WhenContainsAdmin_AdminShouldBeExcluded()
        {
            // Arrange
            var accounts = new List<Account>
    {
        new Account { AccountId = 1, Role = Roles.Admin },
        new Account { AccountId = 2, Role = Roles.Customer }
    };

            _mockAccountRepo.Setup(r => r.GetAllAccountWithProfiles(null, null))
                            .ReturnsAsync(accounts);

            _mockAccountProfileRepo.Setup(r => r.GetAllAsync())
                            .ReturnsAsync(new List<AccountProfile>());

            // ✅ Sửa: mock mapper với It.IsAny<object>()
            _mockMapper.Setup(m => m.Map<List<ViewAccount>>(It.IsAny<object>()))
                       .Returns((object src) =>
                       {
                           var accs = (src as IEnumerable<Account>)?.ToList() ?? new List<Account>();
                           return accs.Select(a => new ViewAccount
                           {
                               AccountId = a.AccountId,
                               Role = a.Role.ToString()
                           }).ToList();
                       });

            // Act
            var result = await _service.GetAllAccountAsync(10, 1, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items); // chỉ còn Customer
            Assert.DoesNotContain(result.Items, a => a.Role == Roles.Admin.ToString());
        }


        [Fact]
        public async Task GetAllAccountAsync_ShouldPassStatusAndRoleToRepository()
        {
            // Arrange
            var status = AccountStatus.ACTIVE;
            var role = Roles.Staff;

            _mockAccountRepo
                .Setup(r => r.GetAllAccountWithProfiles(It.IsAny<AccountStatus?>(), It.IsAny<Roles?>()))
                .ReturnsAsync(new List<Account>
                {
            new Account { AccountId = 1, Role = Roles.Staff, Status = AccountStatus.ACTIVE, Email = "staff@test.com" }
                });

            _mockAccountProfileRepo
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<AccountProfile>());

            // ✅ Mock mapper đúng kiểu IEnumerable<Account>
            _mockMapper
                .Setup(m => m.Map<List<ViewAccount>>(It.IsAny<object>()))
                .Returns((object src) =>
                {
                    var accounts = (src as IEnumerable<Account>)?.ToList() ?? new List<Account>();
                    return accounts.Select(a => new ViewAccount
                    {
                        AccountId = a.AccountId,
                        Email = a.Email,
                        Role = a.Role.ToString(),
                        Status = a.Status.ToString()
                    }).ToList();
                });

            // Act
            var result = await _service.GetAllAccountAsync(10, 1, status, role);

            // Assert
            _mockAccountRepo.Verify(r => r.GetAllAccountWithProfiles(status, role), Times.Once);
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Staff", result.Items.First().Role);
            Assert.Equal("ACTIVE", result.Items.First().Status);
        }


        [Fact]
        public async Task GetAllAccountAsync_WhenExceptionThrown_ReturnsNull()
        {
            // Arrange
            _mockAccountRepo.Setup(r => r.GetAllAccountWithProfiles(null, null))
                            .ThrowsAsync(new Exception("DB Error"));

            // Act
            var result = await _service.GetAllAccountAsync(10, 1, null, null);

            // Assert
            Assert.Null(result);
        }

        //search account by email
        [Fact]
        public async Task GetAccountByEmail_ShouldReturnViewAccount_WhenAccountExists()
        {
            // Arrange
            string email = "found@mail.com";
            var account = new Account
            {
                AccountId = 1,
                Email = email,
                Role = Roles.Customer,
            };

            var mappedView = new ViewAccount
            {
                AccountId = 1,
                Email = email,
                Role = "Customer",
            };

            _mockAccountRepo.Setup(r => r.GetByEmail(email))
                            .ReturnsAsync(account);

            _mockMapper.Setup(m => m.Map<ViewAccount>(account))
                       .Returns(mappedView);

            // Act
            var result = await _service.GetAccountByEmail(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            Assert.Equal("Customer", result.Role);
            _mockAccountRepo.Verify(r => r.GetByEmail(email), Times.Once);
        }

        [Fact]
        public async Task GetAccountByEmail_ShouldReturnNull_WhenAccountNotFound()
        {
            // Arrange
            string email = "missing@mail.com";
            _mockAccountRepo.Setup(r => r.GetByEmail(email))
                            .ReturnsAsync((Account?)null);

            // Act
            var result = await _service.GetAccountByEmail(email);

            // Assert
            Assert.Null(result);
            _mockAccountRepo.Verify(r => r.GetByEmail(email), Times.Once);
        }

        [Fact]
        public async Task GetAccountByEmail_ShouldReturnNull_WhenExceptionThrown()
        {
            // Arrange
            string email = "error@mail.com";
            _mockAccountRepo.Setup(r => r.GetByEmail(email))
                            .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetAccountByEmail(email);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAccountByEmail_ShouldReturnNull_WhenEmailIsEmpty()
        {
            // Arrange
            string email = "";

            // Act
            var result = await _service.GetAccountByEmail(email);

            // Assert
            Assert.Null(result);
            _mockAccountRepo.Verify(r => r.GetByEmail(It.IsAny<string>()), Times.Never);
        }

    }
}
