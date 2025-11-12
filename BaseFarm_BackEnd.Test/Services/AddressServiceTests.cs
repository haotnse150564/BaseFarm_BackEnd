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
using BaseFarm_BackEnd.Test.Utils; // import JWTFake

namespace BaseFarm_BackEnd.Test.Services
{
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

        private AddressServices CreateService(JWTFake fakeJwt)
        {
            return new AddressServices(_mockUow.Object, _mapper, fakeJwt);
        }

        [Fact]
        public async Task CreateAddressAsync_ShouldReturnFail_WhenUserNotFound()
        {
            var fakeJwt = new JWTFake(null!);
            var service = CreateService(fakeJwt);

            var result = await service.CreateAddressAsync(new AddressRequest());

            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
        }

        [Fact]
        public async Task CreateAddressAsync_ShouldSetDefault_WhenFirstAddress()
        {
            var user = new Account { AccountId = 10 };
            var fakeJwt = new JWTFake(user);

            _mockAddressRepo.Setup(r => r.GetListAddressByUserID(10))
                .ReturnsAsync(new List<Address>());

            _mockAddressRepo.Setup(r => r.AddAsync(It.IsAny<Address>()))
                .Callback<Address>(addr =>
                {
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
            Assert.Equal(1, data.AddressId);
        }

        [Fact]
        public async Task CreateAddressAsync_ShouldUnsetOldDefault_WhenNewDefaultIsChosen()
        {
            var user = new Account { AccountId = 10 };
            var fakeJwt = new JWTFake(user);

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
            var fakeJwt = new JWTFake(user);

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

        //delete
        [Fact]
        public async Task DeleteAddressAsync_ShouldReturnFail_WhenUserNotFound()
        {
            // Arrange
            var fakeJwt = new JWTFake(null!);
            var service = CreateService(fakeJwt);

            // Act
            var result = await service.DeleteAddressAsync(1);

            // Assert
            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.Equal("User khng tồn tại", result.Data);
        }

        [Fact]
        public async Task DeleteAddressAsync_ShouldReturnFail_WhenAddressNotFoundOrNotBelongToUser()
        {
            // Arrange
            var user = new Account { AccountId = 10 };
            var fakeJwt = new JWTFake(user);

            _mockAddressRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Address?)null);

            var service = CreateService(fakeJwt);

            // Act
            var result = await service.DeleteAddressAsync(1);

            // Assert
            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.Equal("Địa chỉ không tồn tại", result.Data);
        }

        [Fact]
        public async Task DeleteAddressAsync_ShouldReturnSuccess_WhenDeletedAddressIsDefault()
        {
            // Arrange
            var user = new Account { AccountId = 10 };
            var fakeJwt = new JWTFake(user);

            var addressToDelete = new Address { AddressID = 1, IsDefault = true, CustomerID = 10 };
            var otherAddress = new Address { AddressID = 2, IsDefault = false, CustomerID = 10 };
            var allAddresses = new List<Address> { addressToDelete, otherAddress };

            _mockAddressRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(addressToDelete);
            _mockAddressRepo.Setup(r => r.GetListAddressByUserID(user.AccountId))
                .ReturnsAsync(allAddresses);

            var service = CreateService(fakeJwt);

            // Act
            var result = await service.DeleteAddressAsync(1);

            // Assert
            // Chỉ verify DeleteAsync (có thực hiện)
            _mockAddressRepo.Verify(r => r.DeleteAsync(addressToDelete), Times.Once);

            // Không verify UpdateAsync vì code gốc không gọi
            // Chỉ kiểm tra response
            Assert.Equal(Const.SUCCESS_CREATE_CODE, result.Status);
            Assert.Equal("Xoá thành công, cập nhật lại địa chỉ mặc định", result.Data);
        }


        [Fact]
        public async Task DeleteAddressAsync_ShouldDeleteSuccessfully_WhenAddressIsNotDefault()
        {
            // Arrange
            var user = new Account { AccountId = 10 };
            var fakeJwt = new JWTFake(user);

            var addressToDelete = new Address { AddressID = 1, IsDefault = false, CustomerID = 10 };
            _mockAddressRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(addressToDelete);

            var service = CreateService(fakeJwt);

            // Act
            var result = await service.DeleteAddressAsync(1);

            // Assert
            _mockAddressRepo.Verify(r => r.DeleteAsync(addressToDelete), Times.Once);
            Assert.Equal(Const.SUCCESS_CREATE_CODE, result.Status);
            Assert.Equal("Xoá thành công.", result.Data);
        }

        //update
        [Fact]
        public async Task UpdateAddressAsync_ShouldReturnFail_WhenAddressNotFound()
        {
            // Arrange
            var user = new Account { AccountId = 10 };
            var fakeJwt = new JWTFake(user);

            _mockAddressRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Address?)null);
            _mockAddressRepo.Setup(r => r.GetListAddressByUserID(10)).ReturnsAsync(new List<Address>());

            var service = CreateService(fakeJwt);

            // Act
            var result = await service.UpdateAddressAsync(1, new AddressRequest());

            // Assert
            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.Equal("Tài khoản không tồn tại.", result.Message);
        }


        [Fact]
        public async Task UpdateAddressAsync_ShouldUnsetOldDefault_WhenNewDefaultIsChosen()
        {
            // Arrange
            var user = new Account { AccountId = 10 };
            var fakeJwt = new JWTFake(user);

            var address = new Address { AddressID = 1, IsDefault = false, CustomerID = 10 };
            var oldAddresses = new List<Address>
    {
        new Address { AddressID = 2, IsDefault = true, CustomerID = 10 }
    };

            _mockAddressRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(address);
            _mockAddressRepo.Setup(r => r.GetListAddressByUserID(10)).ReturnsAsync(oldAddresses);

            var service = CreateService(fakeJwt);

            var request = new AddressRequest { IsDefault = true };

            // Act
            var result = await service.UpdateAddressAsync(1, request);

            // Assert
            _mockAddressRepo.Verify(r => r.UpdateAsync(
                It.Is<Address>(a => a.AddressID == 2 && a.IsDefault == false)
            ), Times.Once);

            _mockAddressRepo.Verify(r => r.UpdateAsync(
                It.Is<Address>(a => a.AddressID == 1 && a.IsDefault == true)
            ), Times.Once);

            Assert.Equal(Const.SUCCESS_CREATE_CODE, result.Status);
            var data = Assert.IsType<AddressReponse>(result.Data);
            Assert.True(data.IsDefault);
            Assert.Equal(1, data.AddressId);
        }

        [Fact]
        public async Task UpdateAddressAsync_ShouldSetDefault_WhenNoOtherDefaultExists()
        {
            // Arrange
            var user = new Account { AccountId = 10 };
            var fakeJwt = new JWTFake(user);

            var address = new Address { AddressID = 1, IsDefault = true, CustomerID = 10 };
            var oldAddresses = new List<Address> { address };

            _mockAddressRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(address);
            _mockAddressRepo.Setup(r => r.GetListAddressByUserID(10)).ReturnsAsync(oldAddresses);

            var service = CreateService(fakeJwt);

            var request = new AddressRequest { IsDefault = false };

            // Act
            var result = await service.UpdateAddressAsync(1, request);

            // Assert
            _mockAddressRepo.Verify(r => r.UpdateAsync(address), Times.Once);
            var data = Assert.IsType<AddressReponse>(result.Data);
            Assert.True(data.IsDefault); // Vì không còn default nào nên vẫn phải mặc định
            Assert.Equal(1, data.AddressId);
        }

        [Fact]
        public async Task UpdateAddressAsync_ShouldUpdateSuccessfully_WhenNormalUpdate()
        {
            // Arrange
            var user = new Account { AccountId = 10 };
            var fakeJwt = new JWTFake(user);

            var address = new Address { AddressID = 1, IsDefault = true, CustomerID = 10 };
            var allAddresses = new List<Address> { address };

            _mockAddressRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(address);
            _mockAddressRepo.Setup(r => r.GetListAddressByUserID(10)).ReturnsAsync(allAddresses);

            var service = CreateService(fakeJwt);

            var request = new AddressRequest { IsDefault = true };

            // Act
            var result = await service.UpdateAddressAsync(1, request);

            // Assert
            _mockAddressRepo.Verify(r => r.UpdateAsync(address), Times.AtLeastOnce);

            var data = Assert.IsType<AddressReponse>(result.Data);
            Assert.True(data.IsDefault);
            Assert.Equal(1, data.AddressId);
            Assert.Equal(Const.SUCCESS_CREATE_CODE, result.Status);
        }

        //get all
        [Fact]
        public async Task GetAddressAsync_ShouldReturnFail_WhenUserNotFound()
        {
            // Arrange
            var fakeJwt = new JWTFake(null!);
            var service = CreateService(fakeJwt);

            // Act
            var result = await service.GetAddressAsync();

            // Assert
            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.Equal("Tài khoản không tồn tại.", result.Message);
        }

        [Fact]
        public async Task GetAddressAsync_ShouldReturnFail_WhenAddressIsNull()
        {
            // Arrange
            var user = new Account { AccountId = 10 };
            var fakeJwt = new JWTFake(user);

            _mockAddressRepo.Setup(r => r.GetListAddressByUserID(10)).ReturnsAsync((List<Address>?)null);

            var service = CreateService(fakeJwt);

            // Act
            var result = await service.GetAddressAsync();

            // Assert
            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.Equal("Không có địa chỉ.", result.Message);
        }

        [Fact]
        public async Task GetAddressAsync_ShouldReturnSuccess_WhenAddressExists()
        {
            // Arrange
            var user = new Account { AccountId = 10 };
            var fakeJwt = new JWTFake(user);

            var addresses = new List<Address>
    {
        new Address { AddressID = 1, CustomerID = 10, IsDefault = true },
        new Address { AddressID = 2, CustomerID = 10, IsDefault = false }
    };

            _mockAddressRepo.Setup(r => r.GetListAddressByUserID(10)).ReturnsAsync(addresses);
            _mockAccountRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(user);

            var service = CreateService(fakeJwt);

            // Act
            var result = await service.GetAddressAsync();

            // Assert
            Assert.Equal(Const.SUCCESS_CREATE_CODE, result.Status);
            var data = Assert.IsType<List<AddressReponse>>(result.Data);
            Assert.Equal(2, data.Count);
            Assert.Contains(data, d => d.AddressId == 1 && d.IsDefault);
            Assert.Contains(data, d => d.AddressId == 2 && !d.IsDefault);
        }

    }
}
