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
                           //StartDate = r.StartDate,
                           //EndDate = r.EndDate,
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

        // --- UPDATE FARM ACTIVITY TESTS ---
        // UC1: farmActivityId không tồn tại → FAIL
        [Fact]
        public async Task UpdateFarmActivityAsync_FarmActivityNotFound_ShouldFail()
        {
            _mockUow.Setup(u => u.farmActivityRepository).Returns(_mockFarmActivityRepo.Object);
            _mockFarmActivityRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
                                 .ReturnsAsync((FarmActivity)null);

            var service = CreateService();
            var request = CreateValidRequest(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now));

            var result = await service.UpdateFarmActivityAsync(
                1,
                request,
                ActivityType.Fertilization,
                FarmActivityStatus.ACTIVE
            );

            Assert.Equal(-1, result.Status);
            Assert.Equal("Get Data Fail", result.Message);

        }


        // UC2: StartDate > EndDate → FAIL
        [Fact]
        public async Task UpdateFarmActivityAsync_StartAfterEnd_ShouldFail()
        {
            var farmActivity = new FarmActivity
            {
                //StartDate = DateOnly.FromDateTime(DateTime.Now),
                //EndDate = DateOnly.FromDateTime(DateTime.Now),
                Status = FarmActivityStatus.ACTIVE
            };

            // Mock UnitOfWork và repository
            _mockUow.Setup(u => u.farmActivityRepository).Returns(_mockFarmActivityRepo.Object);
            _mockFarmActivityRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(farmActivity);

            // Tạo request với Start > End (giá trị hợp lệ nhưng muốn fail date)
            var request = new FarmActivityRequest
            {
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            // Tạo service mock, override CheckDate trả false
            var serviceMock = new Mock<FarmActivityServices>(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _mockFarmActivityRepo.Object,
                _mockInventory.Object
            )
            { CallBase = true };

            serviceMock.Setup(s => s.CheckDate(It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>())).Returns(false);

            var result = await serviceMock.Object.UpdateFarmActivityAsync(
                1,
                request,
                ActivityType.Fertilization,
                FarmActivityStatus.ACTIVE
            );

            Assert.Equal(-1, result.Status);
            Assert.Equal("Start date or end date is wrong required!", result.Message);
        }


        // UC3: StartDate ≤ EndDate → SUCCESS
        [Fact]
        public async Task UpdateFarmActivityAsync_ValidUpdate_ShouldSucceed()
        {
            var farmActivity = new FarmActivity
            {
                //StartDate = DateOnly.FromDateTime(DateTime.Now),
                //EndDate = DateOnly.FromDateTime(DateTime.Now),
                Status = FarmActivityStatus.ACTIVE
            };

            // Mock UnitOfWork trả repository
            _mockUow.Setup(u => u.farmActivityRepository).Returns(_mockFarmActivityRepo.Object);
            _mockFarmActivityRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(farmActivity);
            // Update the return value to match the expected Task<int> type
            _mockFarmActivityRepo.Setup(r => r.UpdateAsync(It.IsAny<FarmActivity>())).ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<FarmActivityView>(It.IsAny<FarmActivity>())).Returns(new FarmActivityView());

            var request = new FarmActivityRequest
            {
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2))
            };

            var service = CreateService();
            var result = await service.UpdateFarmActivityAsync(
                1,
                request,
                ActivityType.Harvesting,
                FarmActivityStatus.IN_PROGRESS
            );

            Assert.Equal(1, result.Status);
            Assert.NotNull(result.Data);
        }


        // UC4: Validate StartDate hoặc EndDate null → FAIL
        [Theory]
        [InlineData(true, false)]  // StartDate invalid simulated
        [InlineData(false, true)]  // EndDate invalid simulated
        public async Task UpdateFarmActivityAsync_InvalidDates_ShouldFail(bool invalidStart, bool invalidEnd)
        {
            var farmActivity = new FarmActivity
            {
                //StartDate = DateOnly.FromDateTime(DateTime.Now),
                //EndDate = DateOnly.FromDateTime(DateTime.Now),
                Status = FarmActivityStatus.ACTIVE
            };

            _mockUow.Setup(u => u.farmActivityRepository).Returns(_mockFarmActivityRepo.Object);
            _mockFarmActivityRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(farmActivity);

            // Tạo request với giá trị hợp lệ, để không null
            var request = new FarmActivityRequest
            {
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                EndDate = DateOnly.FromDateTime(DateTime.Now)
            };

            var serviceMock = new Mock<FarmActivityServices>(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _mockFarmActivityRepo.Object,
                _mockInventory.Object
            )
            { CallBase = true };

            // Mock CheckDate để simulate validation fail theo từng trường hợp
            serviceMock.Setup(s => s.CheckDate(
                It.IsAny<DateOnly?>(),
                It.IsAny<DateOnly?>()
            )).Returns(false);

            var result = await serviceMock.Object.UpdateFarmActivityAsync(1, request, ActivityType.Fertilization, FarmActivityStatus.ACTIVE);

            Assert.Equal(-1, result.Status);
            Assert.Equal("Start date or end date is wrong required!", result.Message);
        }

        //change status
        [Fact]
        public async Task ChangeFarmActivityStatusAsync_FarmActivityNotFound_ShouldFail()
        {
            _mockUow.Setup(u => u.farmActivityRepository).Returns(_mockFarmActivityRepo.Object);
            _mockFarmActivityRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((FarmActivity)null);

            var service = CreateService();
            var result = await service.ChangeFarmActivityStatusAsync(1);

            Assert.Equal(-1, result.Status); // Const.FAIL_READ_CODE
            Assert.Equal("Get Data Fail", result.Message); // Const.FAIL_READ_MSG
        }

        [Fact]
        public async Task ChangeFarmActivityStatusAsync_ActiveToDeactivated_ShouldSucceed()
        {
            var farmActivity = new FarmActivity
            {
                Status = FarmActivityStatus.ACTIVE
            };

            _mockUow.Setup(u => u.farmActivityRepository).Returns(_mockFarmActivityRepo.Object);
            _mockFarmActivityRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(farmActivity);
            _mockFarmActivityRepo.Setup(r => r.UpdateAsync(It.IsAny<FarmActivity>())).ReturnsAsync(1);
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = CreateService();
            var result = await service.ChangeFarmActivityStatusAsync(1);

            Assert.Equal(1, result.Status); // Const.SUCCESS_UPDATE_CODE
            Assert.Equal("Update Data Success", result.Message);
            Assert.Equal(FarmActivityStatus.DEACTIVATED, farmActivity.Status);
        }

        [Fact]
        public async Task ChangeFarmActivityStatusAsync_DeactivatedToActive_ShouldSucceed()
        {
            var farmActivity = new FarmActivity
            {
                Status = FarmActivityStatus.DEACTIVATED
            };

            _mockUow.Setup(u => u.farmActivityRepository).Returns(_mockFarmActivityRepo.Object);
            _mockFarmActivityRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(farmActivity);
            _mockFarmActivityRepo.Setup(r => r.UpdateAsync(It.IsAny<FarmActivity>())).ReturnsAsync(1);
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = CreateService();
            var result = await service.ChangeFarmActivityStatusAsync(1);

            Assert.Equal(1, result.Status); // Const.SUCCESS_UPDATE_CODE
            Assert.Equal("Update Data Success", result.Message); // Const.SUCCESS_UPDATE_MSG
            Assert.Equal(FarmActivityStatus.ACTIVE, farmActivity.Status);
        }

        //complete
        // UC1: farmActivity không tồn tại → FAIL
        [Fact]
        public async Task CompleteFarmActivity_FarmActivityNotFound_ShouldFail()
        {
            _mockUow.Setup(u => u.farmActivityRepository).Returns(_mockFarmActivityRepo.Object);
            _mockFarmActivityRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((FarmActivity)null);

            var service = CreateService();
            var result = await service.CompleteFarmActivity(1, null);

            Assert.Equal(-1, result.Status); // Const.FAIL_READ_CODE
            Assert.Equal("Not Found Farm Activity", result.Message);
        }

        // UC2: farmActivity Status != IN_PROGRESS → FAIL
        [Fact]
        public async Task CompleteFarmActivity_StatusNotInProgress_ShouldFail()
        {
            var farmActivity = new FarmActivity { Status = FarmActivityStatus.ACTIVE };
            _mockUow.Setup(u => u.farmActivityRepository).Returns(_mockFarmActivityRepo.Object);
            _mockFarmActivityRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(farmActivity);

            var service = CreateService();
            var result = await service.CompleteFarmActivity(1, null);

            Assert.Equal(-1, result.Status);
            Assert.Equal("Farm Activity Already Completed or do not used by any Schedule", result.Message);
        }

        // UC3: IN_PROGRESS, ActivityType != Harvesting, SaveChanges thành công → SUCCESS
        [Fact]
        public async Task CompleteFarmActivity_NonHarvesting_ShouldSucceed()
        {
            var farmActivity = new FarmActivity
            {
                Status = FarmActivityStatus.IN_PROGRESS,
                ActivityType = ActivityType.Fertilization
            };

            _mockUow.Setup(u => u.farmActivityRepository).Returns(_mockFarmActivityRepo.Object);
            _mockFarmActivityRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(farmActivity);
            _mockFarmActivityRepo.Setup(r => r.UpdateAsync(It.IsAny<FarmActivity>())).ReturnsAsync(1);
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = CreateService();
            var result = await service.CompleteFarmActivity(1, null);

            Assert.Equal(1, result.Status);
            Assert.Equal("Update Data Success", result.Message);
            Assert.Equal(FarmActivityStatus.COMPLETED, farmActivity.Status);
        }

        // UC4: IN_PROGRESS, ActivityType != Harvesting, SaveChanges fail → FAIL
        [Fact]
        public async Task CompleteFarmActivity_NonHarvesting_SaveFail_ShouldFail()
        {
            var farmActivity = new FarmActivity
            {
                Status = FarmActivityStatus.IN_PROGRESS,
                ActivityType = ActivityType.Fertilization
            };

            _mockUow.Setup(u => u.farmActivityRepository).Returns(_mockFarmActivityRepo.Object);
            _mockFarmActivityRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(farmActivity);
            _mockFarmActivityRepo.Setup(r => r.UpdateAsync(It.IsAny<FarmActivity>())).ReturnsAsync(1);
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(-1);

            var service = CreateService();
            var result = await service.CompleteFarmActivity(1, null);

            Assert.Equal(-1, result.Status);
            Assert.Equal("Update Data Fail", result.Message);
        }

        // UC5: IN_PROGRESS, Harvesting, GetProductWillHarves = null → FAIL
        [Fact]
        public async Task CompleteFarmActivity_Harvesting_ProductNull_ShouldFail()
        {
            var farmActivity = new FarmActivity
            {
                Status = FarmActivityStatus.IN_PROGRESS,
                ActivityType = ActivityType.Harvesting,
                FarmActivitiesId = 1
            };

            _mockUow.Setup(u => u.farmActivityRepository).Returns(_mockFarmActivityRepo.Object);
            _mockFarmActivityRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(farmActivity);
            _mockFarmActivityRepo.Setup(r => r.UpdateAsync(It.IsAny<FarmActivity>())).ReturnsAsync(1);
            _mockFarmActivityRepo.Setup(r => r.GetProductWillHarves(It.IsAny<long>())).ReturnsAsync((List<Product>)null);

            var service = CreateService();
            var result = await service.CompleteFarmActivity(1, null);

            Assert.Equal(-1, result.Status);
            Assert.Equal("Get Data Fail", result.Message);
        }

        // UC6: IN_PROGRESS, Harvesting, có product, SaveChanges thành công → SUCCESS
        [Fact]
        public async Task CompleteFarmActivity_Harvesting_ShouldSucceed()
        {
            var farmActivity = new FarmActivity
            {
                Status = FarmActivityStatus.IN_PROGRESS,
                ActivityType = ActivityType.Harvesting,
                FarmActivitiesId = 1
            };
            var products = new List<Product>
    {
        new Product { ProductId = 1, StockQuantity = 0 }
    };
            var schedule = new Schedule { Quantity = 10 };

            _mockUow.Setup(u => u.farmActivityRepository).Returns(_mockFarmActivityRepo.Object);
            _mockUow.Setup(u => u.scheduleRepository).Returns(Mock.Of<IScheduleRepository>());
            var mockScheduleRepo = Mock.Get(_mockUow.Object.scheduleRepository);
            mockScheduleRepo.Setup(s => s.GetByCropId(It.IsAny<long>())).ReturnsAsync(schedule);

            _mockFarmActivityRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(farmActivity);
            _mockFarmActivityRepo.Setup(r => r.UpdateAsync(It.IsAny<FarmActivity>())).ReturnsAsync(1);
            _mockFarmActivityRepo.Setup(r => r.GetProductWillHarves(It.IsAny<long>())).ReturnsAsync(products);
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = CreateService();
            var result = await service.CompleteFarmActivity(1, null);

            Assert.Equal(1, result.Status);
            Assert.Equal("Update Data Success", result.Message);
            Assert.Equal(FarmActivityStatus.COMPLETED, farmActivity.Status);
            Assert.Equal(10, products[0].StockQuantity);
        }

        // UC7: IN_PROGRESS, Harvesting, có product, SaveChanges fail → FAIL
        [Fact]
        public async Task CompleteFarmActivity_Harvesting_SaveFail_ShouldFail()
        {
            var farmActivity = new FarmActivity
            {
                Status = FarmActivityStatus.IN_PROGRESS,
                ActivityType = ActivityType.Harvesting,
                FarmActivitiesId = 1
            };
            var products = new List<Product>
    {
        new Product { ProductId = 1, StockQuantity = 0 }
    };
            var schedule = new Schedule { Quantity = 10 };

            _mockUow.Setup(u => u.farmActivityRepository).Returns(_mockFarmActivityRepo.Object);
            _mockUow.Setup(u => u.scheduleRepository).Returns(Mock.Of<IScheduleRepository>());
            var mockScheduleRepo = Mock.Get(_mockUow.Object.scheduleRepository);
            mockScheduleRepo.Setup(s => s.GetByCropId(It.IsAny<long>())).ReturnsAsync(schedule);

            _mockFarmActivityRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(farmActivity);
            _mockFarmActivityRepo.Setup(r => r.UpdateAsync(It.IsAny<FarmActivity>())).ReturnsAsync(1);
            _mockFarmActivityRepo.Setup(r => r.GetProductWillHarves(It.IsAny<long>())).ReturnsAsync(products);
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(-1);

            var service = CreateService();
            var result = await service.CompleteFarmActivity(1, null);

            Assert.Equal(-1, result.Status);
            Assert.Equal("Update Data Fail", result.Message);
        }

    }
}
