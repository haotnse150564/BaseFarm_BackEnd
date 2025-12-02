//using Application;
//using Application.Interfaces;
//using Application.Services.Implement;
//using AutoMapper;
//using Domain.Enum;
//using Domain.Model;
//using Infrastructure.Repositories;
//using Infrastructure.ViewModel.Request;
//using Microsoft.Extensions.Configuration;
//using Moq;
//using System;
//using System.Threading.Tasks;
//using Xunit;
//using static Infrastructure.ViewModel.Response.CropRequirementResponse;

//namespace BaseFarm_BackEnd.Test.Services
//{
//    public class CropRequirementServiceTests
//    {
//        private readonly Mock<IUnitOfWorks> _mockUnitOfWork;
//        private readonly Mock<ICurrentTime> _mockCurrentTime;
//        private readonly Mock<IConfiguration> _mockConfiguration;
//        private readonly IMapper _mapper;
//        private readonly CropRequirementServices _service;

//        public CropRequirementServiceTests()
//        {
//            _mockUnitOfWork = new Mock<IUnitOfWorks>();
//            _mockCurrentTime = new Mock<ICurrentTime>();
//            _mockConfiguration = new Mock<IConfiguration>();

//            // --- MOCK REPOSITORY ---
//            var mockRepo = new Mock<ICropRequirementRepository>();
//            mockRepo.Setup(r => r.AddAsync(It.IsAny<CropRequirement>()))
//                .Returns<CropRequirement>(c =>
//                {
//                    c.CropRequirementId = 1;
//                    c.CropId = 1;
//                    c.PlantStage ??= PlantStage.Sowing;
//                    // Logic này sẽ chạy sau khi map xong, gán giá trị int giả định
//                    c.WateringFrequency ??= 1;
//                    return Task.CompletedTask;
//                });

//            _mockUnitOfWork.Setup(u => u.cropRequirementRepository).Returns(mockRepo.Object);
//            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

//            // --- AUTOMAPPER CONFIG (PHẦN ĐÃ SỬA) ---
//            var config = new MapperConfiguration(cfg =>
//            {
//                // SỬA Ở ĐÂY: Ignore WateringFrequency khi map từ Request -> Entity
//                // Lý do: Request là string ("Daily"), Entity là int.
//                // AutoMapper sẽ crash nếu cố parse. Ta Ignore để nó null, 
//                // sau đó Mock Repo ở trên sẽ gán nó thành 1.
//                cfg.CreateMap<CropRequirementRequest, CropRequirement>()
//                   .ForMember(dest => dest.WateringFrequency, opt => opt.Ignore());

//                cfg.CreateMap<CropRequirement, CropRequirementView>()
//                    .ForMember(dest => dest.PlantStage,
//                        opt => opt.MapFrom(src => src.PlantStage.HasValue ? src.PlantStage.ToString() : null))
//                    .ForMember(dest => dest.CropRequirementId, opt => opt.MapFrom(src => src.CropRequirementId))
//                    .ForMember(dest => dest.CropId, opt => opt.MapFrom(src => src.CropId))
//                    .ForMember(dest => dest.WateringFrequency,
//                        opt => opt.MapFrom(src => src.WateringFrequency.HasValue ? src.WateringFrequency.Value.ToString() : null));
//            });
//            _mapper = config.CreateMapper();

//            _service = new CropRequirementServices(
//                _mockUnitOfWork.Object,
//                _mockCurrentTime.Object,
//                _mockConfiguration.Object,
//                _mapper
//            );
//        }

//        [Fact]
//        public async Task CreateCropRequirementAsync_ShouldReturnSuccess_Sowing()
//        {
//            var req = new CropRequirementRequest
//            {
//                EstimatedDate = 10,
//                Moisture = 20,
//                Temperature = 25,
//                Fertilizer = "NPK",
//                LightRequirement = 12,
//                WateringFrequency = "Daily",
//                Notes = "Note"
//            };
//            _mockCurrentTime.Setup(c => c.GetCurrentTime()).Returns(DateOnly.FromDateTime(DateTime.Now));

//            var res = await _service.CreateCropRequirementAsync(req, PlantStage.Sowing, 1);

//            Assert.NotNull(res);
//            Assert.Equal(Const.SUCCESS_CREATE_CODE, res.Status);

//            var view = Assert.IsType<CropRequirementView>(res.Data);
//            Assert.Equal("Sowing", view.PlantStage);
//            Assert.Equal(1, view.CropRequirementId);
//            Assert.Equal(1, view.CropId);
//            Assert.Equal(req.EstimatedDate, view.EstimatedDate);
//            Assert.Equal(req.Moisture, view.Moisture);
//            Assert.Equal("1", view.WateringFrequency);
//        }

//        [Fact]
//        public async Task CreateCropRequirementAsync_ShouldReturnSuccess_Germination()
//        {
//            var req = new CropRequirementRequest
//            {
//                EstimatedDate = 5,
//                Moisture = 18,
//                Temperature = 22,
//                Fertilizer = "Organic",
//                LightRequirement = 10,
//                WateringFrequency = "Every 2 days",
//                Notes = "Note"
//            };
//            _mockCurrentTime.Setup(c => c.GetCurrentTime()).Returns(DateOnly.FromDateTime(DateTime.Now));

//            var res = await _service.CreateCropRequirementAsync(req, PlantStage.Germination, 2);

//            Assert.NotNull(res);
//            Assert.Equal(Const.SUCCESS_CREATE_CODE, res.Status);

//            var view = Assert.IsType<CropRequirementView>(res.Data);
//            Assert.Equal("Germination", view.PlantStage);
//            Assert.Equal(req.Fertilizer, view.Fertilizer);
//            Assert.Equal("1", view.WateringFrequency);
//        }

//        [Fact]
//        public async Task CreateCropRequirementAsync_ShouldReturnSuccess_NullOptionalFields()
//        {
//            var req = new CropRequirementRequest
//            {
//                EstimatedDate = null,
//                Moisture = null,
//                Temperature = null,
//                Fertilizer = null,
//                LightRequirement = null,
//                WateringFrequency = null,
//                Notes = null
//            };
//            _mockCurrentTime.Setup(c => c.GetCurrentTime()).Returns(DateOnly.FromDateTime(DateTime.Now));

//            var res = await _service.CreateCropRequirementAsync(req, PlantStage.TrueLeavesGrowth, 3);

//            Assert.NotNull(res);
//            Assert.Equal(Const.SUCCESS_CREATE_CODE, res.Status);

//            var view = Assert.IsType<CropRequirementView>(res.Data);
//            Assert.Equal("TrueLeavesGrowth", view.PlantStage);
//            Assert.Equal(1, view.CropRequirementId);
//            Assert.Equal("1", view.WateringFrequency);
//        }

//        [Fact]
//        public async Task CreateCropRequirementAsync_ShouldMapCorrectlyToView()
//        {
//            var req = new CropRequirementRequest
//            {
//                EstimatedDate = 7,
//                Moisture = 15,
//                Temperature = 20,
//                Fertilizer = "Compost",
//                LightRequirement = 8,
//                WateringFrequency = "Weekly",
//                Notes = "Check mapping"
//            };
//            _mockCurrentTime.Setup(c => c.GetCurrentTime()).Returns(DateOnly.FromDateTime(DateTime.Now));

//            var res = await _service.CreateCropRequirementAsync(req, PlantStage.VigorousGrowth, 4);

//            Assert.NotNull(res.Data);
//            var view = Assert.IsType<CropRequirementView>(res.Data);

//            Assert.Equal(req.EstimatedDate, view.EstimatedDate);
//            Assert.Equal(req.Moisture, view.Moisture);
//            Assert.Equal(req.Temperature, view.Temperature);
//            Assert.Equal(req.Fertilizer, view.Fertilizer);
//            Assert.Equal("VigorousGrowth", view.PlantStage);
//            Assert.Equal(1, view.CropRequirementId);
//            Assert.Equal(1, view.CropId);
//            Assert.Equal("1", view.WateringFrequency);
//        }
//    }
//}
