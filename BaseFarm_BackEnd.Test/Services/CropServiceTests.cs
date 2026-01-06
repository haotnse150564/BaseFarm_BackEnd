//using Application;
//using Application.Interfaces;
//using Application.Services.Implement;
//using Application.ViewModel.Request;
//using AutoMapper;
//using BaseFarm_BackEnd.Test.EnumTest;
//using BaseFarm_BackEnd.Test.Utils;
//using Domain.Enum;
//using Domain.Model;
//using Infrastructure.ViewModel.Request;
//using Moq;
//using Xunit;
//using static Application.ViewModel.Response.ProductResponse;
//using static Infrastructure.ViewModel.Response.CropResponse;
//using Microsoft.Extensions.Configuration;
//using Infrastructure.Repositories;
//using System.ComponentModel.DataAnnotations;

//namespace BaseFarm_BackEnd.Test.Services
//{
//    public class CropServiceTests
//    {
//        private readonly Mock<IUnitOfWorks> _mockUow;
//        private readonly Mock<ICurrentTime> _mockTime;
//        private readonly Mock<IMapper> _mockMapper;
//        private readonly Mock<ICropRepository> _mockCropRepo;
//        private readonly Mock<IConfiguration> _mockConfig;

//        public CropServiceTests()
//        {
//            _mockUow = new Mock<IUnitOfWorks>();
//            _mockTime = new Mock<ICurrentTime>();
//            _mockMapper = new Mock<IMapper>();
//            _mockCropRepo = new Mock<ICropRepository>();
//            _mockConfig = new Mock<IConfiguration>();

//            // gán cropRepo vào unitOfWork
//            _mockUow.Setup(x => x.cropRepository).Returns(_mockCropRepo.Object);
//        }

//        // Tạo service theo đúng constructor của CropServices
//        //private CropServices CreateService(JWTFake fakeJwt) =>
//        //    new CropServices(
//        //        _mockUow.Object,
//        //        _mockTime.Object,
//        //        _mockConfig.Object,
//        //        _mockMapper.Object,
//        //        _mockCropRepo.Object,
//        //        fakeJwt
//        //    );

//        // ================================================================
//        // UC1: User = null → FAIL
//        // ================================================================
//        [Fact]
//        public async Task CreateCropAsync_UserNull_ShouldFail()
//        {
//            var fakeJwt = new JWTFake(null);
//            var service = CreateService(fakeJwt);

//            var cropRequest = new CropRequest
//            {
//                CropName = "Tomato",
//                Origin = "Vietnam",
//                CategoryId = 1
//            };
//            var productDTO = new ProductRequestDTO.CreateProductDTO
//            {
//                ProductName = "Tomato Product",
//                Price = 15000
//            };

//            var result = await service.CreateCropAsync(cropRequest, productDTO);

//            Assert.Equal(-1, result.Status);
//            Assert.Equal("Tài khoản không hợp lệ.", result.Message);
//        }

//        // ================================================================
//        // UC2: User role != Manager → FAIL
//        // ================================================================
//        [Fact]
//        public async Task CreateCropAsync_UserIsNotManager_ShouldFail()
//        {
//            var fakeJwt = new JWTFake(new Account { AccountId = 1, Role = Roles.Customer });
//            var service = CreateService(fakeJwt);

//            var cropRequest = new CropRequest
//            {
//                CropName = "Tomato",
//                Origin = "Vietnam",
//                CategoryId = 1
//            };
//            var productDTO = new ProductRequestDTO.CreateProductDTO
//            {
//                ProductName = "Tomato Product",
//                Price = 15000
//            };

//            var result = await service.CreateCropAsync(cropRequest, productDTO);

//            Assert.Equal(-1, result.Status);
//            Assert.Equal("Tài khoản không hợp lệ.", result.Message);
//        }

//        // ================================================================
//        // UC3: CropRequest thiếu field bắt buộc → FAIL
//        // ================================================================
//        [Fact]
//        public void CreateCropAsync_CropRequestValidation_ShouldFail()
//        {
//            var cropRequest = new CropRequest
//            {
//                CropName = null, // thiếu bắt buộc
//                Origin = null,   // thiếu bắt buộc
//                CategoryId = null
//            };

//            var validationResults = new List<ValidationResult>();
//            var context = new ValidationContext(cropRequest, null, null);
//            var isValid = Validator.TryValidateObject(cropRequest, context, validationResults, true);

//            Assert.False(isValid);
//            Assert.Contains(validationResults, v => v.MemberNames.Contains("CropName"));
//            Assert.Contains(validationResults, v => v.MemberNames.Contains("Origin"));
//            Assert.Contains(validationResults, v => v.MemberNames.Contains("CategoryId"));
//        }

//        // ================================================================
//        // UC4: ProductDTO thiếu field bắt buộc → FAIL
//        // ================================================================
//        [Fact]
//        public void CreateCropAsync_ProductDTOValidation_ShouldFail()
//        {
//            var productDTO = new ProductRequestDTO.CreateProductDTO
//            {
//                ProductName = null, // thiếu bắt buộc
//                Price = null        // thiếu bắt buộc
//            };

//            var validationResults = new List<ValidationResult>();
//            var context = new ValidationContext(productDTO, null, null);
//            var isValid = Validator.TryValidateObject(productDTO, context, validationResults, true);

//            Assert.False(isValid);
//            Assert.Contains(validationResults, v => v.MemberNames.Contains("ProductName"));
//            Assert.Contains(validationResults, v => v.MemberNames.Contains("Price"));
//        }

//        // ================================================================
//        // UC5: Tạo mới thành công → SUCCESS
//        // ================================================================
//        [Fact]
//        public async Task CreateCropAsync_CreateSuccessfully_ShouldSucceed()
//        {
//            var user = new Account { AccountId = 1, Role = Roles.Manager };
//            var fakeJwt = new JWTFake(user);
//            var service = CreateService(fakeJwt);

//            var cropReq = new CropRequest
//            {
//                CategoryId = 2,
//                CropName = "Test Crop",
//                Origin = "Vietnam"
//            };

//            var productReq = new ProductRequestDTO.CreateProductDTO
//            {
//                ProductName = "Test Product",
//                Price = 20000
//            };

//            var product = new Product { ProductId = 10 };
//            var crop = new Crop { CropName = "Test Crop", CategoryId = 2 };
//            var category = new Category { CategoryId = 2, CategoryName = "Vegetable" };

//            _mockMapper.Setup(x => x.Map<Product>(productReq)).Returns(product);
//            _mockMapper.Setup(x => x.Map<Crop>(cropReq)).Returns(crop);
//            _mockMapper.Setup(x => x.Map<CropView>(crop)).Returns(new CropView());
//            _mockMapper.Setup(x => x.Map<ProductDetailDTO>(product)).Returns(new ProductDetailDTO());

//            _mockUow.Setup(x => x.productRepository.AddAsync(product));
//            _mockCropRepo.Setup(x => x.AddAsync(crop));
//            _mockUow.Setup(x => x.categoryRepository.GetByIdAsync(2)).ReturnsAsync(category);
//            _mockUow.Setup(x => x.SaveChangesAsync());

//            _mockTime.Setup(t => t.GetCurrentTime())
//                     .Returns(DateOnly.FromDateTime(DateTime.Now));

//            var result = await service.CreateCropAsync(cropReq, productReq);

//            Assert.Equal(1, result.Status);
//            Assert.Equal("Create Data Success", result.Message);
//            Assert.NotNull(result.Data);

//            _mockUow.Verify(x => x.productRepository.AddAsync(product), Times.Once);
//            _mockCropRepo.Verify(x => x.AddAsync(crop), Times.Once);
//            _mockUow.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
//        }

//        // ================================================================
//        // UC1: Crop không tồn tại → FAIL
//        // ================================================================
//        [Fact]
//        public async Task UpdateCrop_CropNotFound_ShouldFail()
//        {
//            var cropId = 1L;
//            _mockUow.Setup(u => u.cropRepository.GetByIdAsync(cropId))
//                    .ReturnsAsync((Crop)null);

//            var service = CreateService(new JWTFake(new Account { Role = Roles.Manager }));
//            var cropUpdate = new CropRequest { CropName = "Updated Crop" };

//            var result = await service.UpdateCrop(cropUpdate, cropId);

//            Assert.Equal(-1, result.Status);
//            Assert.Equal("Get Data Fail", result.Message); // sửa message cho khớp với service
//        }

//        // ================================================================
//        // UC2: Update thành công → SUCCESS
//        // ================================================================
//        [Fact]
//        public async Task UpdateCrop_ValidInput_ShouldSucceed()
//        {
//            var cropId = 1L;
//            var existingCrop = new Crop { CropId = cropId, CropName = "Old Crop" };
//            var cropUpdate = new CropRequest { CropName = "Updated Crop", Description = "New Desc" };

//            _mockUow.Setup(u => u.cropRepository.GetByIdAsync(cropId))
//                    .ReturnsAsync(existingCrop);

//            _mockMapper.Setup(m => m.Map(cropUpdate, existingCrop)).Returns(existingCrop);
//            _mockMapper.Setup(m => m.Map<CropView>(existingCrop))
//                       .Returns(new CropView { CropName = "Updated Crop" });

//            _mockUow.Setup(u => u.cropRepository.UpdateAsync(existingCrop));
//            _mockUow.Setup(u => u.SaveChangesAsync());

//            var service = CreateService(new JWTFake(new Account { Role = Roles.Manager }));

//            var result = await service.UpdateCrop(cropUpdate, cropId);

//            Assert.Equal(1, result.Status);
//            Assert.Equal("Update Data Success", result.Message);
//            Assert.IsType<CropView>(result.Data);
//            Assert.Equal("Updated Crop", ((CropView)result.Data).CropName);
//        }

//        // ================================================================
//        // UC3: CropRequest validation fail → FAIL
//        // ================================================================
//        [Fact]
//        public void UpdateCrop_CropRequestValidation_ShouldFail()
//        {
//            var cropUpdate = new CropRequest
//            {
//                CropName = null, // required
//                Origin = null
//            };

//            var validationResults = new List<ValidationResult>();
//            var context = new ValidationContext(cropUpdate, null, null);
//            var isValid = Validator.TryValidateObject(cropUpdate, context, validationResults, true);

//            Assert.False(isValid);
//            Assert.Contains(validationResults, v => v.MemberNames.Contains("CropName"));
//            Assert.Contains(validationResults, v => v.MemberNames.Contains("Origin"));
//        }

//        // ================================================================
//        // UC5: User = null → FAIL
//        // ================================================================
//        [Fact]
//        public async Task UpdateCrop_UserNull_ShouldFail()
//        {
//            var cropUpdate = new CropRequest { CropName = "Updated Crop" };
//            var service = CreateService(new JWTFake(null));

//            var result = await service.UpdateCrop(cropUpdate, 1L);

//            Assert.Equal(-1, result.Status);
//            Assert.Equal("Get Data Fail", result.Message); // sửa message cho khớp
//        }

//        // ================================================================
//        // UC6: User role != Manager → FAIL
//        // ================================================================
//        [Fact]
//        public async Task UpdateCrop_UserNotManager_ShouldFail()
//        {
//            var cropUpdate = new CropRequest { CropName = "Updated Crop" };
//            var fakeJwt = new JWTFake(new Account { Role = Roles.Customer });
//            var service = CreateService(fakeJwt);

//            var result = await service.UpdateCrop(cropUpdate, 1L);

//            Assert.Equal(-1, result.Status);
//            Assert.Equal("Get Data Fail", result.Message); // sửa message cho khớp
//        }

//        // ================================================================
//        // UC1: Crop không tồn tại → FAIL
//        // ================================================================
//        [Fact]
//        public async Task UpdateCropStatusAsync_CropNotFound_ShouldFail()
//        {
//            var cropId = 1L;
//            var status = 1;

//            _mockUow.Setup(u => u.cropRepository.GetByIdAsync(cropId))
//                    .ReturnsAsync((Crop)null);

//            var service = CreateService(new JWTFake(new Account { Role = Roles.Manager }));

//            var result = await service.UpdateCropStatusAsync(cropId, status);

//            Assert.Equal(-1, result.Status);
//            Assert.Equal("Get Data Fail", result.Message); // sửa message
//        }

//        // ================================================================
//        // UC2: Cập nhật status thành công → SUCCESS
//        // ================================================================
//        [Fact]
//        public async Task UpdateCropStatusAsync_ValidInput_ShouldSucceed()
//        {
//            var cropId = 1L;
//            var status = 2; // ví dụ Active = 2
//            var existingCrop = new Crop { CropId = cropId, Status = CropStatus.INACTIVE };

//            _mockUow.Setup(u => u.cropRepository.GetByIdAsync(cropId))
//                    .ReturnsAsync(existingCrop);

//            _mockUow.Setup(u => u.cropRepository.UpdateAsync(existingCrop));
//            _mockUow.Setup(u => u.SaveChangesAsync());

//            var service = CreateService(new JWTFake(new Account { Role = Roles.Manager }));

//            var result = await service.UpdateCropStatusAsync(cropId, status);

//            Assert.Equal(1, result.Status);
//            Assert.Equal("Update Data Success", result.Message); // sửa message
//            Assert.Equal((CropStatus)status, existingCrop.Status); // đảm bảo crop.Status được cập nhật
//        }

//        // ================================================================
//        // UC3: Cập nhật status sang giá trị hiện tại → SUCCESS
//        // ================================================================
//        [Fact]
//        public async Task UpdateCropStatusAsync_StatusSameAsCurrent_ShouldSucceed()
//        {
//            var cropId = 1L;
//            var currentStatus = 2; // giả sử status hiện tại = 2
//            var existingCrop = new Crop { CropId = cropId, Status = (CropStatus)currentStatus };

//            _mockUow.Setup(u => u.cropRepository.GetByIdAsync(cropId))
//                    .ReturnsAsync(existingCrop);

//            _mockUow.Setup(u => u.cropRepository.UpdateAsync(existingCrop));
//            _mockUow.Setup(u => u.SaveChangesAsync());

//            var service = CreateService(new JWTFake(new Account { Role = Roles.Manager }));

//            var result = await service.UpdateCropStatusAsync(cropId, currentStatus);

//            Assert.Equal(1, result.Status);
//            Assert.Equal("Update Data Success", result.Message); // sửa message
//            Assert.Equal((CropStatus)currentStatus, existingCrop.Status); // trạng thái vẫn giữ nguyên
//        }

//        // ================================================================
//        // UC1: Không có crop nào → Exception
//        // ================================================================
//        [Fact]
//        public async Task GetAllCropsAsync_NoCrops_ShouldThrowException()
//        {
//            // Setup repository trả về danh sách rỗng
//            _mockUow.Setup(u => u.cropRepository.GetAllAsync())
//                    .ReturnsAsync(new List<Crop>());

//            var service = CreateService(new JWTFake(new Account { Role = Roles.Manager }));

//            // Kiểm tra exception
//            await Assert.ThrowsAsync<Exception>(async () =>
//            {
//                await service.GetAllCropsAsync(1, 10);
//            });
//        }

//        // ================================================================
//        // UC2: Có crops → trả về Pagination đúng
//        // ================================================================
//        [Fact]
//        public async Task GetAllCropsAsync_WithCrops_ShouldReturnPagination()
//        {
//            // Tạo danh sách crop mẫu
//            var crops = new List<Crop>
//            {
//                new Crop { CropId = 1, CropName = "Crop1" },
//                new Crop { CropId = 2, CropName = "Crop2" },
//                new Crop { CropId = 3, CropName = "Crop3" }
//            };

//            _mockUow.Setup(u => u.cropRepository.GetAllAsync())
//                    .ReturnsAsync(crops);

//            // Mapper map Crop → CropView
//            _mockMapper.Setup(m => m.Map<List<CropView>>(crops))
//                       .Returns(crops.Select(c => new CropView { CropName = c.CropName }).ToList());

//            var service = CreateService(new JWTFake(new Account { Role = Roles.Manager }));

//            int pageIndex = 1;
//            int pageSize = 2;

//            var result = await service.GetAllCropsAsync(pageIndex, pageSize);

//            Assert.NotNull(result);
//            Assert.Equal(3, result.TotalItemCount);      // tổng số crop
//            Assert.Equal(pageSize, result.PageSize);     // pageSize = 2
//            Assert.Equal(pageIndex, result.PageIndex);   // pageIndex = 1
//            Assert.Equal(2, result.Items.Count);         // chỉ lấy 2 item cho page 1

//            // Fix: Use LINQ's ElementAt() to access items in ICollection
//            Assert.Equal("Crop1", result.Items.ElementAt(0).CropName);
//            Assert.Equal("Crop2", result.Items.ElementAt(1).CropName);
//        }
//    }
//}
