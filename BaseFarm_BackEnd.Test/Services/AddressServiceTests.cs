using AutoMapper;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application;
using Infrastructure.Repositories;
using Application.Utils;
using Application.Services.Implement;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BaseFarm_BackEnd.Test.Services
{
    public class AddressFakeJWTUtils : JWTUtils
    {
        private readonly Account _user;

        public AddressFakeJWTUtils(Account user = null!)
            : base(Mock.Of<IUnitOfWorks>(), Mock.Of<IConfiguration>(), Mock.Of<IHttpContextAccessor>())
        {
            _user = user;
        }

        public override Task<Account> GetCurrentUserAsync()
        {
            return Task.FromResult(_user);
        }
    }

    public class AddressServiceTests
    {
        private readonly Mock<IUnitOfWorks> _mockUow;
        private readonly Mock<IAddressRepository> _mockAddressRepo;
        private readonly Mock<IAccountRepository> _mockAccountRepo;
        private readonly IMapper _mapper;

        public AddressServiceTests()
        {
            _mockUow = new Mock<IUnitOfWorks>();
            _mockAddressRepo = new Mock<IAddressRepository>();
            _mockAccountRepo = new Mock<IAccountRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddressRequest, Address>();
                cfg.CreateMap<Address, AddressReponse>();
            });

            _mapper = config.CreateMapper();

            _mockUow.Setup(u => u.addressRepository).Returns(_mockAddressRepo.Object);
            _mockUow.Setup(u => u.accountRepository).Returns(_mockAccountRepo.Object);
        }

        private AddressServices CreateService(AddressFakeJWTUtils fakeJwt)
        {
            return new AddressServices(_mockUow.Object, _mapper, fakeJwt);
        }

        [Fact]
        public async Task CreateAddressAsync_ShouldReturnFail_WhenUserNotFound()
        {
            var fakeJwt = new AddressFakeJWTUtils(null!);
            var service = CreateService(fakeJwt);

            var result = await service.CreateAddressAsync(new AddressRequest());

            // Chỉnh sửa: dùng Const.ERROR_EXCEPTION = -4
            Assert.Equal(Const.ERROR_EXCEPTION, result.Status);
        }

        [Fact]
        public async Task CreateAddressAsync_ShouldSetDefault_WhenFirstAddress()
        {
            var user = new Account { AccountId = 10 };
            var fakeJwt = new AddressFakeJWTUtils(user);

            _mockAddressRepo.Setup(r => r.GetListAddressByUserID(10))
                .ReturnsAsync(new List<Address>());

            _mockAddressRepo.Setup(r => r.AddAsync(It.IsAny<Address>()))
                .Callback<Address>(addr =>
                {
                    // giả lập repository gán ID và IsDefault
                    addr.AddressID = 1;
                    addr.IsDefault = true;
                })
                .Returns(Task.CompletedTask);

            _mockAccountRepo.Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(user);

            var service = CreateService(fakeJwt);

            var result = await service.CreateAddressAsync(new AddressRequest());

            var data = Assert.IsType<AddressReponse>(result.Data);
            Assert.True(data.IsDefault);
            Assert.Equal(1, data.AddressId); // kiểm tra ID được set
        }


        [Fact]
        public async Task CreateAddressAsync_ShouldUnsetOldDefault_WhenNewDefaultIsChosen()
        {
            var user = new Account { AccountId = 10 };
            var fakeJwt = new AddressFakeJWTUtils(user);

            var oldAddresses = new List<Address>
            {
                new Address { AddressID = 1, IsDefault = true, CustomerID = 10 }
            };
            _mockAddressRepo.Setup(r => r.GetListAddressByUserID(10))
                .ReturnsAsync(oldAddresses);

            _mockAccountRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(user);

            var service = CreateService(fakeJwt);

            await service.CreateAddressAsync(new AddressRequest { IsDefault = true });

            _mockAddressRepo.Verify(r => r.UpdateAsync(
                It.Is<Address>(a => a.AddressID == 1 && a.IsDefault == false)
            ), Times.Once);
        }

        [Fact]
        public async Task CreateAddressAsync_ShouldNotChangeDefault_WhenNewAddressIsNotDefault()
        {
            var user = new Account { AccountId = 10 };
            var fakeJwt = new AddressFakeJWTUtils(user);

            var oldAddresses = new List<Address>
            {
                new Address { AddressID = 1, IsDefault = true, CustomerID = 10 }
            };
            _mockAddressRepo.Setup(r => r.GetListAddressByUserID(10))
                .ReturnsAsync(oldAddresses);

            _mockAccountRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(user);

            var service = CreateService(fakeJwt);

            await service.CreateAddressAsync(new AddressRequest { IsDefault = false });

            _mockAddressRepo.Verify(r => r.UpdateAsync(It.IsAny<Address>()), Times.Never);
        }
    }
}
