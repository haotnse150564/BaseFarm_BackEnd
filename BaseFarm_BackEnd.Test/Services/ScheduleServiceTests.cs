using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoMapper;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using Application.Services.Implement;
using Application.Interfaces;
using Domain.Enum;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using static Infrastructure.ViewModel.Response.ScheduleResponse;
using Application.Services;
using Application;
using BaseFarm_BackEnd.Test.Utils;
using BaseFarm_BackEnd.Test.EnumTest;

namespace BaseFarm_BackEnd.Test.Services
{
    public class ScheduleServiceTests
    {
        private Mock<IUnitOfWorks> _mockUow;
        private Mock<ICurrentTime> _mockCurrentTime;
        private Mock<IConfiguration> _mockConfig;
        private Mock<IMapper> _mockMapper;
        private Mock<IAccountRepository> _mockAccountRepo;
        private Mock<ICropRepository> _mockCropRepo;
        private Mock<IFarmActivityRepository> _mockFarmActivityRepo;
        private Mock<IFarmRepository> _mockFarmRepo;
        private Mock<IInventoryService> _mockInventory;

        public ScheduleServiceTests()
        {
            _mockUow = new Mock<IUnitOfWorks>();
            _mockCurrentTime = new Mock<ICurrentTime>();
            _mockConfig = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
            _mockAccountRepo = new Mock<IAccountRepository>();
            _mockCropRepo = new Mock<ICropRepository>();
            _mockFarmActivityRepo = new Mock<IFarmActivityRepository>();
            _mockFarmRepo = new Mock<IFarmRepository>();
            _mockInventory = new Mock<IInventoryService>();
        }

        private ScheduleServices CreateService(JWTFake fakeJwt) =>
            new ScheduleServices(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _mockAccountRepo.Object,
                _mockCropRepo.Object,
                _mockFarmActivityRepo.Object,
                _mockFarmRepo.Object,
                fakeJwt,
                _mockInventory.Object
            );

        private ScheduleRequest CreateValidRequest()
        {
            return new ScheduleRequest
            {
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                PlantingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                HarvestDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                StaffId = 2,
                CropId = 1,
                FarmId = 1,
                Quantity = 10,
                Status = Status.ACTIVE
            };
        }

        private Account CreateManager()
        {
            return new Account
            {
                Role = Roles.Manager,
                AccountId = 1,
                AccountProfile = new AccountProfile { Fullname = "Manager" }
            };
        }

        // --- Helper: Mock mapper trả Schedule đầy đủ ---
        private void SetupMapperMock()
        {
            _mockMapper.Setup(m => m.Map<Schedule>(It.IsAny<ScheduleRequest>()))
                .Returns((ScheduleRequest r) => new Schedule
                {
                    StartDate = r.StartDate ?? DateOnly.FromDateTime(DateTime.Now),
                    PlantingDate = r.PlantingDate ?? DateOnly.FromDateTime(DateTime.Now),
                    EndDate = r.EndDate ?? DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    HarvestDate = r.HarvestDate ?? DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                    ManagerId = 1,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                    Quantity = r.Quantity,
                    Status = r.Status,
                    FarmId = r.FarmId,
                    CropId = r.CropId,
                    AssignedTo = r.StaffId
                });
        }

        // UC1: User không phải Manager → FAIL
        [Fact]
        public async Task CreateSchedulesAsync_UserNotManager_ShouldFail()
        {
            var request = CreateValidRequest();
            var user = new Account { Role = Roles.Customer, AccountProfile = new AccountProfile { Fullname = "Customer" } };
            var service = CreateService(new JWTFake(user));

            var result = await service.CreateSchedulesAsync(request);

            Assert.Equal(-1, result.Status);
            Assert.Equal("Tài khoản không hợp lệ.", result.Message);
        }

        // UC2: StartDate trong quá khứ → FAIL (bỏ qua message chi tiết)
        [Fact]
        public async Task CreateSchedulesAsync_StartDateInPast_ShouldFail()
        {
            var manager = CreateManager();
            var request = CreateValidRequest();
            request.StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

            SetupMapperMock();

            var serviceMock = new Mock<ScheduleServices>(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _mockAccountRepo.Object,
                _mockCropRepo.Object,
                _mockFarmActivityRepo.Object,
                _mockFarmRepo.Object,
                new JWTFake(manager),
                _mockInventory.Object
            )
            { CallBase = true };

            serviceMock.Setup(s => s.ValidateScheduleDate(It.IsAny<Schedule>()))
                       .ReturnsAsync((true, "")); // bypass ValidateSchedule

            var result = await serviceMock.Object.CreateSchedulesAsync(request);

            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.NotNull(result.Message); // Chỉ kiểm tra có message thôi, không quan tâm nội dung
        }

        // UC3: PlantingDate trong quá khứ → FAIL
        [Fact]
        public async Task CreateSchedulesAsync_PlantingDateInPast_ShouldFail()
        {
            var manager = CreateManager();
            var request = CreateValidRequest();
            request.PlantingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

            SetupMapperMock();

            var serviceMock = new Mock<ScheduleServices>(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _mockAccountRepo.Object,
                _mockCropRepo.Object,
                _mockFarmActivityRepo.Object,
                _mockFarmRepo.Object,
                new JWTFake(manager),
                _mockInventory.Object
            )
            { CallBase = true };

            serviceMock.Setup(s => s.ValidateScheduleDate(It.IsAny<Schedule>()))
                       .ReturnsAsync((true, "")); // bypass ValidateSchedule

            var result = await serviceMock.Object.CreateSchedulesAsync(request);

            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.NotNull(result.Message); // Bỏ qua nội dung message
        }

        // UC4: ValidateScheduleDate fail → FAIL
        [Fact]
        public async Task CreateSchedulesAsync_ValidateScheduleFail_ShouldFail()
        {
            var manager = CreateManager();
            var request = CreateValidRequest();

            SetupMapperMock();

            var serviceMock = new Mock<ScheduleServices>(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _mockAccountRepo.Object,
                _mockCropRepo.Object,
                _mockFarmActivityRepo.Object,
                _mockFarmRepo.Object,
                new JWTFake(manager),
                _mockInventory.Object
            )
            { CallBase = true };

            serviceMock.Setup(s => s.ValidateScheduleDate(It.IsAny<Schedule>()))
                       .ReturnsAsync((false, "Validate fail message")); // Có message tượng trưng

            var result = await serviceMock.Object.CreateSchedulesAsync(request);

            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.NotNull(result.Message); // Không kiểm tra nội dung cụ thể
        }


        // UC5: Create thành công → SUCCESS
        [Fact]
        public async Task CreateSchedulesAsync_ValidInput_ShouldSucceed()
        {
            var manager = CreateManager();
            var request = CreateValidRequest();

            _mockMapper.Setup(m => m.Map<Schedule>(It.IsAny<ScheduleRequest>()))
                .Returns((ScheduleRequest r) => new Schedule
                {
                    StartDate = r.StartDate,
                    PlantingDate = r.PlantingDate,
                    EndDate = r.EndDate,
                    HarvestDate = r.HarvestDate,
                    ManagerId = 1,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                    Quantity = r.Quantity,
                    Status = r.Status,
                    FarmId = r.FarmId,
                    CropId = r.CropId,
                    AssignedTo = r.StaffId
                });

            _mockCurrentTime.Setup(c => c.GetCurrentTime()).Returns(DateOnly.FromDateTime(DateTime.Now));
            _mockAccountRepo.Setup(a => a.GetByIdAsync(request.StaffId))
                .ReturnsAsync(new Account { AccountProfile = new AccountProfile { Fullname = "Staff B" } });
            _mockUow.Setup(u => u.scheduleRepository.AddAsync(It.IsAny<Schedule>()));
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<ScheduleResponseView>(It.IsAny<Schedule>()))
                .Returns(new ScheduleResponseView());

            var service = CreateService(new JWTFake(manager));

            var result = await service.CreateSchedulesAsync(request);

            Assert.Equal(Const.SUCCESS_CREATE_CODE, result.Status);
            Assert.NotNull(result.Data);
        }

        // -------------------- UPDATE SCHEDULE UC --------------------

        // UC1: User không phải Manager → FAIL
        [Fact]
        public async Task UpdateSchedulesAsync_UserNotManager_ShouldFail()
        {
            var request = CreateValidRequest();
            var user = new Account { Role = Roles.Customer, AccountProfile = new AccountProfile { Fullname = "Customer" } };
            var service = CreateService(new JWTFake(user));

            var result = await service.UpdateSchedulesAsync(1, request);

            Assert.Equal(StatusTest.ERROR_EXCEPTION, result.Status);
            Assert.Equal("Tài khoản không hợp lệ.", result.Message);
        }

        // UC2: StartDate trong quá khứ → FAIL
        [Fact]
        public async Task UpdateSchedulesAsync_StartDateInPast_ShouldFail()
        {
            var manager = CreateManager();
            var request = CreateValidRequest();
            request.StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

            SetupMapperMock();

            var serviceMock = new Mock<ScheduleServices>(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _mockAccountRepo.Object,
                _mockCropRepo.Object,
                _mockFarmActivityRepo.Object,
                _mockFarmRepo.Object,
                new JWTFake(manager),
                _mockInventory.Object
            )
            { CallBase = true };

            // Mock ValidateScheduleDate luôn trả true để test riêng StartDate
            serviceMock.Setup(s => s.ValidateScheduleDate(It.IsAny<Schedule>()))
                       .ReturnsAsync((true, ""));

            // Mock repository trả schedule hiện có
            _mockUow.Setup(u => u.scheduleRepository.GetByIdAsync(It.IsAny<long>()))
                     .ReturnsAsync(new Schedule());

            var result = await serviceMock.Object.UpdateSchedulesAsync(1, request);

            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.NotNull(result.Message);
        }

        // UC3: PlantingDate trong quá khứ → FAIL
        [Fact]
        public async Task UpdateSchedulesAsync_PlantingDateInPast_ShouldFail()
        {
            var manager = CreateManager();
            var request = CreateValidRequest();
            request.PlantingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

            SetupMapperMock();

            var serviceMock = new Mock<ScheduleServices>(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _mockAccountRepo.Object,
                _mockCropRepo.Object,
                _mockFarmActivityRepo.Object,
                _mockFarmRepo.Object,
                new JWTFake(manager),
                _mockInventory.Object
            )
            { CallBase = true };

            serviceMock.Setup(s => s.ValidateScheduleDate(It.IsAny<Schedule>()))
                       .ReturnsAsync((true, ""));

            _mockUow.Setup(u => u.scheduleRepository.GetByIdAsync(It.IsAny<long>()))
                     .ReturnsAsync(new Schedule());

            var result = await serviceMock.Object.UpdateSchedulesAsync(1, request);

            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.NotNull(result.Message);
        }

        // UC4: ValidateScheduleDate fail → FAIL
        [Fact]
        public async Task UpdateSchedulesAsync_ValidateScheduleFail_ShouldFail()
        {
            var manager = CreateManager();
            var request = CreateValidRequest();

            SetupMapperMock();

            var serviceMock = new Mock<ScheduleServices>(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _mockAccountRepo.Object,
                _mockCropRepo.Object,
                _mockFarmActivityRepo.Object,
                _mockFarmRepo.Object,
                new JWTFake(manager),
                _mockInventory.Object
            )
            { CallBase = true };

            // Simulate ValidateSchedule fail
            serviceMock.Setup(s => s.ValidateScheduleDate(It.IsAny<Schedule>()))
                       .ReturnsAsync((false, "Validate fail message"));

            _mockUow.Setup(u => u.scheduleRepository.GetByIdAsync(It.IsAny<long>()))
                     .ReturnsAsync(new Schedule());

            var result = await serviceMock.Object.UpdateSchedulesAsync(1, request);

            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.NotNull(result.Message);
        }

        // UC5: Update thành công → SUCCESS
        [Fact]
        public async Task UpdateSchedulesAsync_ValidInput_ShouldSucceed()
        {
            var manager = CreateManager();
            var request = CreateValidRequest();

            _mockMapper.Setup(m => m.Map<ScheduleRequest, Schedule>(It.IsAny<ScheduleRequest>(), It.IsAny<Schedule>()))
                .Returns((ScheduleRequest r, Schedule s) =>
                {
                    s.StartDate = r.StartDate;
                    s.PlantingDate = r.PlantingDate;
                    s.EndDate = r.EndDate;
                    s.HarvestDate = r.HarvestDate;
                    s.Quantity = r.Quantity;
                    s.Status = r.Status;
                    s.FarmId = r.FarmId;
                    s.CropId = r.CropId;
                    s.AssignedTo = r.StaffId;
                    return s;
                });

            _mockCurrentTime.Setup(c => c.GetCurrentTime()).Returns(DateOnly.FromDateTime(DateTime.Now));
            _mockAccountRepo.Setup(a => a.GetByIdAsync(request.StaffId))
                .ReturnsAsync(new Account { AccountProfile = new AccountProfile { Fullname = "Staff B" } });
            _mockAccountRepo.Setup(a => a.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new Account { AccountProfile = new AccountProfile { Fullname = "Manager" } });

            _mockUow.Setup(u => u.scheduleRepository.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new Schedule { ManagerId = 1 });
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<ScheduleResponseView>(It.IsAny<Schedule>()))
                .Returns(new ScheduleResponseView());

            var service = CreateService(new JWTFake(manager));

            var result = await service.UpdateSchedulesAsync(1, request);

            Assert.Equal(Const.SUCCESS_CREATE_CODE, result.Status);
            Assert.NotNull(result.Data);
        }

        //change status
        // UC1: User không phải Manager → FAIL
        [Fact]
        public async Task ChangeScheduleStatusById_UserNotManager_ShouldFail()
        {
            var user = new Account { Role = Roles.Customer, AccountProfile = new AccountProfile { Fullname = "Customer" } };
            var service = CreateService(new JWTFake(user));

            var result = await service.ChangeScheduleStatusById(1, "ACTIVE");

            Assert.Equal(-1, result.Status); // FAIL_READ_CODE
            Assert.Equal("Tài khoản không hợp lệ.", result.Message);
        }

        // UC2: Schedule không tồn tại → FAIL
        [Fact]
        public async Task ChangeScheduleStatusById_ScheduleNotExist_ShouldFail()
        {
            var manager = CreateManager();
            _mockUow.Setup(u => u.scheduleRepository.GetByIdAsync(It.IsAny<long>()))
                    .ReturnsAsync((Schedule)null);

            var service = CreateService(new JWTFake(manager));

            var result = await service.ChangeScheduleStatusById(999, "ACTIVE");

            Assert.Equal(-1, result.Status); // FAIL_READ_CODE
            Assert.Equal("Lịch trình không tồn tại.", result.Message);
        }

        // UC3: Trạng thái không thay đổi → FAIL
        [Fact]
        public async Task ChangeScheduleStatusById_StatusSame_ShouldFail()
        {
            var manager = CreateManager();
            var schedule = new Schedule { Status = Status.ACTIVE };

            _mockUow.Setup(u => u.scheduleRepository.GetByIdAsync(It.IsAny<long>()))
                    .ReturnsAsync(schedule);

            var service = CreateService(new JWTFake(manager));

            var result = await service.ChangeScheduleStatusById(1, "ACTIVE");

            Assert.Equal(-1, result.Status); // FAIL_READ_CODE
            Assert.Equal("Trạng thái không thay đổi.", result.Message);
        }

        // UC4: ValidateScheduleDate fail → FAIL
        [Fact]
        public async Task ChangeScheduleStatusById_ValidateScheduleFail_ShouldFail()
        {
            var manager = CreateManager();
            var schedule = new Schedule { Status = Status.DEACTIVATED };

            _mockUow.Setup(u => u.scheduleRepository.GetByIdAsync(It.IsAny<long>()))
                    .ReturnsAsync(schedule);

            var serviceMock = new Mock<ScheduleServices>(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _mockAccountRepo.Object,
                _mockCropRepo.Object,
                _mockFarmActivityRepo.Object,
                _mockFarmRepo.Object,
                new JWTFake(manager),
                _mockInventory.Object
            )
            { CallBase = true };

            serviceMock.Setup(s => s.ValidateScheduleDate(It.IsAny<Schedule>()))
                       .ReturnsAsync((false, "Validate fail message"));

            var result = await serviceMock.Object.ChangeScheduleStatusById(1, "ACTIVE");

            Assert.Equal(-1, result.Status); // FAIL_CREATE_CODE
            Assert.NotNull(result.Message);
        }

        // UC5: Change thành công → SUCCESS
        [Fact]
        public async Task ChangeScheduleStatusById_ValidInput_ShouldSucceed()
        {
            var manager = CreateManager();
            var schedule = new Schedule { Status = Status.DEACTIVATED };

            _mockUow.Setup(u => u.scheduleRepository.GetByIdAsync(It.IsAny<long>()))
                    .ReturnsAsync(schedule);
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<ScheduleResponseView>(It.IsAny<Schedule>()))
                       .Returns(new ScheduleResponseView());
            _mockCurrentTime.Setup(c => c.GetCurrentTime()).Returns(DateOnly.FromDateTime(DateTime.Now));

            var serviceMock = new Mock<ScheduleServices>(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _mockAccountRepo.Object,
                _mockCropRepo.Object,
                _mockFarmActivityRepo.Object,
                _mockFarmRepo.Object,
                new JWTFake(manager),
                _mockInventory.Object
            )
            { CallBase = true };

            serviceMock.Setup(s => s.ValidateScheduleDate(It.IsAny<Schedule>()))
                       .ReturnsAsync((true, ""));

            var result = await serviceMock.Object.ChangeScheduleStatusById(1, "ACTIVE");

            Assert.Equal(1, result.Status); // SUCCESS_UPDATE_CODE
            Assert.NotNull(result.Data);
        }

    }
}
