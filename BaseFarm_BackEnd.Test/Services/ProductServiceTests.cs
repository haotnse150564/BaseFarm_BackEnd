using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoMapper;
using Domain.Model;
using static Application.ViewModel.Request.ProductRequestDTO;
using static Application.ViewModel.Response.ProductResponse;
using Application.Interfaces;
using Application.Services.Implement;
using Application.Utils;
using Microsoft.Extensions.Configuration;
using BaseFarm_BackEnd.Test.EnumTest;
using BaseFarm_BackEnd.Test.Utils;
using Application;
using Domain.Enum;
using Infrastructure.Repositories;

namespace BaseFarm_BackEnd.Test.Services
{
    public class ProductServiceTests
    {
        private Mock<IUnitOfWorks> _mockUow;
        private Mock<ICurrentTime> _mockCurrentTime;
        private Mock<IConfiguration> _mockConfig;
        private Mock<IMapper> _mockMapper;

        private ProductServices CreateService(JWTFake fakeJwt) =>
            new ProductServices(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                fakeJwt
            );

        public ProductServiceTests()
        {
            _mockUow = new Mock<IUnitOfWorks>();
            _mockCurrentTime = new Mock<ICurrentTime>();
            _mockConfig = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
        }

        // UC1: Product không tồn tại → FAIL
        [Fact]
        public async Task UpdateProductById_ProductNotFound_ShouldFail()
        {
            long productId = 1;
            var request = new UpdateProductDTO();

            // Tạo mock repository đầy đủ
            var mockProductRepo = new Mock<IProductRepository>();
            mockProductRepo.Setup(r => r.GetProductById(productId)).ReturnsAsync((Product)null);

            var mockCategoryRepo = new Mock<ICategoryRepository>();
            mockCategoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category>());

            _mockUow.Setup(u => u.productRepository).Returns(mockProductRepo.Object);
            _mockUow.Setup(u => u.categoryRepository).Returns(mockCategoryRepo.Object);

            var service = CreateService(new JWTFake(new Account { Role = Roles.Manager }));

            var result = await service.UpdateProductById(productId, request);

            // Match actual service
            Assert.Equal(-1, result.Status);
            Assert.Equal("Get Data Fail", result.Message);
        }


        // UC2: Category không tồn tại → FAIL
        [Fact]
        public async Task UpdateProductById_CategoryNotExist_ShouldFail()
        {
            long productId = 1;
            var request = new UpdateProductDTO { CategoryId = 2 };

            var product = new Product { ProductId = productId };
            var categories = new List<Category>(); // danh sách rỗng → không tồn tại CategoryId=2

            _mockUow.Setup(u => u.productRepository.GetProductById(productId))
                    .ReturnsAsync(product);

            _mockUow.Setup(u => u.categoryRepository.GetAllAsync())
                    .ReturnsAsync(categories);

            var service = CreateService(new JWTFake(new Account { Role = Roles.Manager }));

            var result = await service.UpdateProductById(productId, request);

            Assert.Equal(-1, result.Status);               // Const.FAIL_READ_CODE
            Assert.Equal("Get Data Fail", result.Message); // sửa cho khớp service
        }

        // UC3: Update thành công → SUCCESS
        [Fact]
        public async Task UpdateProductById_ValidInput_ShouldSucceed()
        {
            long productId = 1;
            var request = new UpdateProductDTO { CategoryId = 1 };

            var product = new Product
            {
                ProductId = productId,
                ProductNavigation = new Crop { CropName = "Crop A" },
                StockQuantity = 10
            };

            var categories = new List<Category>
    {
        new Category { CategoryId = 1, CategoryName = "Cat 1" }
    };

            _mockUow.Setup(u => u.productRepository.GetProductById(productId))
                    .ReturnsAsync(product);

            _mockUow.Setup(u => u.categoryRepository.GetAllAsync())
                    .ReturnsAsync(categories);

            _mockMapper.Setup(m => m.Map(request, product)).Returns(product);

            _mockUow.Setup(u => u.productRepository.UpdateAsync(product));
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _mockMapper.Setup(m => m.Map<ProductDetailDTO>(product))
                       .Returns(new ProductDetailDTO
                       {
                           CropName = "Crop A",
                           StockQuantity = 10
                       });

            var service = CreateService(new JWTFake(new Account { Role = Roles.Manager }));

            var result = await service.UpdateProductById(productId, request);

            Assert.Equal(1, result.Status);               // Const.SUCCESS_READ_CODE
            Assert.Equal("Update Data Success", result.Message); // sửa cho khớp service
            Assert.NotNull(result.Data);
        }

        //status update
        // ================================================================
        // UC1: Product không tồn tại → FAIL
        // ================================================================
        [Fact]
        public async Task ChangeProductStatusById_ProductNotFound_ShouldFail()
        {
            long productId = 1;

            var mockProductRepo = new Mock<IProductRepository>();
            mockProductRepo.Setup(r => r.GetProductById(productId)).ReturnsAsync((Product)null);

            _mockUow.Setup(u => u.productRepository).Returns(mockProductRepo.Object);

            var service = CreateService(new JWTFake(new Account { Role = Roles.Manager }));

            var result = await service.ChangeProductStatusById(productId);

            Assert.Equal(-1, result.Status); // FAIL_READ_CODE
            Assert.Equal("Get Data Fail", result.Message);
        }

        // ================================================================
        // UC2: Product ACTIVE → DEACTIVED → SUCCESS
        // ================================================================
        [Fact]
        public async Task ChangeProductStatusById_ActiveToDeactived_ShouldSucceed()
        {
            long productId = 1;
            var product = new Product { ProductId = productId, Status = ProductStatus.ACTIVE };

            var mockProductRepo = new Mock<IProductRepository>();
            mockProductRepo.Setup(r => r.GetProductById(productId)).ReturnsAsync(product);

            _mockUow.Setup(u => u.productRepository).Returns(mockProductRepo.Object);
            _mockUow.Setup(u => u.productRepository.UpdateAsync(product));
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = CreateService(new JWTFake(new Account { Role = Roles.Manager }));

            var result = await service.ChangeProductStatusById(productId);

            Assert.Equal(1, result.Status); // SUCCESS_READ_CODE
            Assert.Equal("Get Data Success", result.Message);
            Assert.Equal(ProductStatus.DEACTIVED, product.Status);
        }

        // ================================================================
        // UC3: Product DEACTIVED → ACTIVE → SUCCESS
        // ================================================================
        [Fact]
        public async Task ChangeProductStatusById_DeactivedToActive_ShouldSucceed()
        {
            long productId = 1;
            var product = new Product { ProductId = productId, Status = ProductStatus.DEACTIVED };

            var mockProductRepo = new Mock<IProductRepository>();
            mockProductRepo.Setup(r => r.GetProductById(productId)).ReturnsAsync(product);

            _mockUow.Setup(u => u.productRepository).Returns(mockProductRepo.Object);
            _mockUow.Setup(u => u.productRepository.UpdateAsync(product));
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = CreateService(new JWTFake(new Account { Role = Roles.Manager }));

            var result = await service.ChangeProductStatusById(productId);

            Assert.Equal(1, result.Status); // SUCCESS_READ_CODE
            Assert.Equal("Get Data Success", result.Message);
            Assert.Equal(ProductStatus.ACTIVE, product.Status);
        }
    }
}
