using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Application.Services.Implement;
using BaseFarm_BackEnd.Test.Utils;
using Domain.Model;
using static Infrastructure.ViewModel.Request.FeedbackRequest;
using Application;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BaseFarm_BackEnd.Test.Services
{
    public class FeedbackServiceTests
    {
        private readonly Mock<IUnitOfWorks> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ICurrentTime> _mockCurrentTime;
        private readonly Mock<IConfiguration> _mockConfig;

        public FeedbackServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWorks>();
            _mockMapper = new Mock<IMapper>();
            _mockCurrentTime = new Mock<ICurrentTime>();
            _mockConfig = new Mock<IConfiguration>();
        }

        private FeedbackServices CreateService(JWTFake jwt)
        {
            return new FeedbackServices(
                _mockUnitOfWork.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                jwt
            );
        }

        [Fact]
        public async Task CreateFeedbackAsync_ShouldReturnError_WhenUserIsNull()
        {
            // Arrange
            var fakeJwt = new JWTFake(null!);
            var service = CreateService(fakeJwt);

            var request = new CreateFeedbackDTO { OrderDetailId = 1 };

            // Act
            var result = await service.CreateFeedbackAsync(request);

            // Assert
            Assert.Equal(Const.ERROR_EXCEPTION, result.Status);
            Assert.Contains("User not found", result.Message);
        }

        [Fact]
        public async Task CreateFeedbackAsync_ShouldReturnFail_WhenOrderDetailNotFound()
        {
            // Arrange
            var user = new Account { AccountId = 10 };
            var fakeJwt = new JWTFake(user);

            _mockUnitOfWork.Setup(u => u.orderDetailRepository.GetByIdAsync(1))
                .ReturnsAsync((OrderDetail?)null);

            var service = CreateService(fakeJwt);

            var request = new CreateFeedbackDTO { OrderDetailId = 1 };

            // Act
            var result = await service.CreateFeedbackAsync(request);

            // Assert
            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.Equal("Order detail not found.", result.Message);
        }

        [Fact]
        public async Task CreateFeedbackAsync_ShouldCreateSuccessfully()
        {
            // Arrange
            var user = new Account { AccountId = 10 };
            var fakeJwt = new JWTFake(user);

            var orderDetail = new OrderDetail { OrderDetailId = 1 };
            _mockUnitOfWork.Setup(u => u.orderDetailRepository.GetByIdAsync(1))
                .ReturnsAsync(orderDetail);

            var feedback = new Feedback();
            var request = new CreateFeedbackDTO
            {
                OrderDetailId = 1,
                Comment = "Great product",
                Rating = 5
            };

            _mockMapper.Setup(m => m.Map<Feedback>(request)).Returns(feedback);

            _mockUnitOfWork.Setup(u => u.feedbackRepository.AddAsync(It.IsAny<Feedback>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(fakeJwt);

            // Act
            var result = await service.CreateFeedbackAsync(request);

            // Assert
            _mockUnitOfWork.Verify(u => u.feedbackRepository.AddAsync(It.IsAny<Feedback>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);

            Assert.Equal(Const.SUCCESS_CREATE_CODE, result.Status);
            Assert.Equal("Feedback created!", result.Message);
        }

        [Fact]
        public async Task CreateFeedbackAsync_ShouldCatchException_AndReturnError()
        {
            // Arrange
            var user = new Account { AccountId = 10 };
            var fakeJwt = new JWTFake(user);
            var service = CreateService(fakeJwt);

            _mockUnitOfWork.Setup(u => u.orderDetailRepository.GetByIdAsync(It.IsAny<long>()))
                .ThrowsAsync(new Exception("DB crashed"));

            var request = new CreateFeedbackDTO { OrderDetailId = 1 };

            // Act
            var result = await service.CreateFeedbackAsync(request);

            // Assert
            Assert.Equal(Const.ERROR_EXCEPTION, result.Status);
            Assert.Contains("DB crashed", result.Message);
        }
    }
}
