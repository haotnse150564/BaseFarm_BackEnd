using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Domain.Enum;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using Application.Services.Implement;
using Application.Interfaces;
using Application.Services;
using Application;
using Infrastructure.Repositories;
using WebAPI.Services;
using static Infrastructure.ViewModel.Response.FarmActivityResponse;

namespace BaseFarm_BackEnd.Test.Services
{
    public class FarmActivityServiceTests
    {
        private readonly Mock<IUnitOfWorks> _mockUow;
        private readonly Mock<ICurrentTime> _mockCurrentTime;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IFarmActivityRepository> _mockFarmActivityRepo;
        private readonly Mock<IInventoryService> _mockInventory;

        public FarmActivityServiceTests()
        {
            _mockUow = new Mock<IUnitOfWorks>();
            _mockCurrentTime = new Mock<ICurrentTime>();
            _mockConfig = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
            _mockFarmActivityRepo = new Mock<IFarmActivityRepository>();
            _mockInventory = new Mock<IInventoryService>();
        }

        private FarmActivityServices CreateService() =>
            new FarmActivityServices(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _mockFarmActivityRepo.Object,
                _mockInventory.Object
            );

        private FarmActivityRequest CreateValidRequest(DateOnly start, DateOnly end)
        {
            return new FarmActivityRequest
            {
                StartDate = start,
                EndDate = end
            };
        }

        // --- COMMON MOCK SETUP ---
        private void SetupMapperAndRepoMocks(FarmActivityRequest request)
        {
            // Mapper trả FarmActivity dựa trên request
            _mockMapper.Setup(m => m.Map<FarmActivity>(It.IsAny<FarmActivityRequest>()))
                       .Returns((FarmActivityRequest r) => new FarmActivity
                       {
                           StartDate = r.StartDate,
                           EndDate = r.EndDate,
                           Status = FarmActivityStatus.ACTIVE
                       });

            _mockMapper.Setup(m => m.Map<FarmActivityView>(It.IsAny<FarmActivity>()))
                       .Returns(new FarmActivityView());

            _mockFarmActivityRepo.Setup(f => f.AddAsync(It.IsAny<FarmActivity>()))
                                 .Returns(Task.CompletedTask);

            // Gắn repository vào unitOfWork
            _mockUow.Setup(u => u.farmActivityRepository).Returns(_mockFarmActivityRepo.Object);

            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Current time trước StartDate để check date pass
            _mockCurrentTime.Setup(c => c.GetCurrentTime())
                            .Returns(request.StartDate.Value.AddDays(-1));
        }

        // UC1: StartDate > EndDate → FAIL
        [Fact]
        public async Task CreateFarmActivityAsync_StartAfterEnd_ShouldFail()
        {
            var start = DateOnly.FromDateTime(DateTime.Now.AddDays(2));
            var end = start.AddDays(-1); // Start > End
            var request = CreateValidRequest(start, end);

            SetupMapperAndRepoMocks(request);

            var service = CreateService();
            var result = await service.CreateFarmActivityAsync(request, ActivityType.Fertilization);

            Assert.Equal(-1, result.Status);
            Assert.Equal("Start date or end date is wrong!", result.Message);
        }

        // UC2: StartDate = EndDate → SUCCESS
        [Fact]
        public async Task CreateFarmActivityAsync_SameStartEnd_ShouldSucceed()
        {
            var start = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var end = start; // Start = End
            var request = CreateValidRequest(start, end);

            SetupMapperAndRepoMocks(request);

            var service = CreateService();
            var result = await service.CreateFarmActivityAsync(request, ActivityType.Fertilization);

            Assert.Equal(1, result.Status);
            Assert.NotNull(result.Data);
        }

        // UC3: StartDate < EndDate, khoảng cách <=7 → SUCCESS
        [Fact]
        public async Task CreateFarmActivityAsync_ValidDates_ShouldSucceed()
        {
            var start = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var end = start.AddDays(5); // Khoảng cách 5 ngày <= 7
            var request = CreateValidRequest(start, end);

            SetupMapperAndRepoMocks(request);

            var service = CreateService();
            var result = await service.CreateFarmActivityAsync(request, ActivityType.Fertilization);

            Assert.Equal(1, result.Status);
            Assert.NotNull(result.Data);
        }

        // UC4: StartDate < EndDate, khoảng cách >7 → FAIL
        [Fact]
        public async Task CreateFarmActivityAsync_TooLongDuration_ShouldFail()
        {
            var start = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var end = start.AddDays(10); // Khoảng cách 10 ngày > 7
            var request = CreateValidRequest(start, end);

            SetupMapperAndRepoMocks(request);

            // Tạo Service mock lại CheckDate trả false
            var serviceMock = new Mock<FarmActivityServices>(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _mockFarmActivityRepo.Object,
                _mockInventory.Object
            )
            { CallBase = true };

            serviceMock.Setup(s => s.CheckDate(It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>()))
                       .Returns(false);

            var result = await serviceMock.Object.CreateFarmActivityAsync(request, ActivityType.Fertilization);

            Assert.Equal(-1, result.Status);
            Assert.Equal("Start date or end date is wrong!", result.Message);
        }

    }
}
