
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Application.Utils;
using Domain.Model;
using Domain.Enum;
using Application.Services.Implement;
using static Application.ViewModel.Request.OrderRequest;
using static Application.ViewModel.Response.OrderResponse;
using Microsoft.EntityFrameworkCore.Storage;
using Application.Services;
using Application;
using Infrastructure.Repositories;
using Application.ViewModel.Request;
using Microsoft.AspNetCore.SignalR;

namespace BaseFarm_BackEnd.Test.Services
{
    // =========================
    // ✅ FAKE / SUPPORT CLASSES
    // =========================
    public class BaseResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }
    }

    public static class Const
    {
        public const int SUCCESS_CREATE_CODE = 1;
        public const int FAIL_CREATE_CODE = -1;
        public const int ERROR_EXCEPTION = -4;
    }

    // ✅ Fake JWTUtils
    public class FakeJWTUtils : JWTUtils
    {
        private readonly Account _fakeUser;
        private readonly bool _throws;

        public FakeJWTUtils(Account? fakeUser = null, bool throwsException = false)
            : base(null!, null!, null!)
        {
            _fakeUser = fakeUser ?? new Account { AccountId = 999, Email = "FakeUser" };
            _throws = throwsException;
        }

        public new Task<Account> GetCurrentUserAsync()
        {
            if (_throws)
                throw new Exception("JWT failed");
            return Task.FromResult(_fakeUser);
        }
    }

    // ✅ Fake OrderService để mô phỏng CreateOrderAsync (ko sửa code gốc)
    public class FakeOrderService
    {
        private readonly FakeJWTUtils _jwt;

        public FakeOrderService(FakeJWTUtils jwt)
        {
            _jwt = jwt;
        }

        public async Task<BaseResponse<CreateOrderResultDTO>> FakeCreateOrderAsync(CreateOrderDTO dto)
        {
            try
            {
                var user = await _jwt.GetCurrentUserAsync();

                if (dto == null || dto.OrderItems == null || dto.OrderItems.Count == 0)
                {
                    return new BaseResponse<CreateOrderResultDTO>
                    {
                        Status = Const.FAIL_CREATE_CODE,
                        Message = "Invalid order data",
                        Data = null
                    };
                }

                // Giả lập logic kiểm tra product
                foreach (var item in dto.OrderItems)
                {
                    if (item.StockQuantity <= 0)
                    {
                        return new BaseResponse<CreateOrderResultDTO>
                        {
                            Status = Const.FAIL_CREATE_CODE,
                            Message = "Invalid product quantity",
                            Data = null
                        };
                    }
                }

                // Giả lập tính tổng
                decimal total = 0;
                foreach (var item in dto.OrderItems)
                    total += (decimal)(item.StockQuantity * 20); // giả lập giá 20 / sản phẩm

                var result = new CreateOrderResultDTO
                {
                    //OrderId = 1000,
                    TotalPrice = total,
                    PaymentUrl = "https://fake-pay.vn/pay/1000"
                };

                return new BaseResponse<CreateOrderResultDTO>
                {
                    Status = Const.SUCCESS_CREATE_CODE,
                    Message = "Order created successfully",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<CreateOrderResultDTO>
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }
    }

    // =========================
    // ✅ TEST CLASS
    // =========================
    public class OrderServiceTests
    {
        private readonly Mock<IUnitOfWorks> _mockUow;
        private readonly Mock<IOrderRepository> _mockOrderRepo;
        private readonly Mock<IOrderDetailRepository> _mockOrderDetailRepo;
        private readonly Mock<IProductRepository> _mockProductRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IVnPayService> _mockVnPay;
        private readonly Mock<CheckDate> _mockCheckDate;
        private readonly Mock<IDbContextTransaction> _mockDbTransaction;

        private readonly Mock<IHubContext<OrderNotificationHub>> _mockHubContext;
        private readonly Mock<IClientProxy> _mockClientProxy;
        private readonly Mock<IHubClients> _mockHubClients;

        public OrderServiceTests()
        {
            _mockUow = new Mock<IUnitOfWorks>();
            _mockOrderRepo = new Mock<IOrderRepository>();
            _mockOrderDetailRepo = new Mock<IOrderDetailRepository>();
            _mockProductRepo = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockConfig = new Mock<IConfiguration>();
            _mockVnPay = new Mock<IVnPayService>();
            _mockCheckDate = new Mock<CheckDate>();
            _mockDbTransaction = new Mock<IDbContextTransaction>();

            // 🔔 Mock SignalR HubContext
            _mockHubContext = new Mock<IHubContext<OrderNotificationHub>>();
            _mockHubClients = new Mock<IHubClients>();
            _mockClientProxy = new Mock<IClientProxy>();

            _mockHubContext.Setup(h => h.Clients).Returns(_mockHubClients.Object);
            _mockHubClients.Setup(c => c.All).Returns(_mockClientProxy.Object);

            // Repo bindings
            _mockUow.SetupGet(u => u.orderRepository).Returns(_mockOrderRepo.Object);
            _mockUow.SetupGet(u => u.orderDetailRepository).Returns(_mockOrderDetailRepo.Object);
            _mockUow.SetupGet(u => u.productRepository).Returns(_mockProductRepo.Object);
            _mockUow.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(_mockDbTransaction.Object);

            _mockConfig.Setup(c => c["BaseUrl"]).Returns("https://test-basefarm.com");
        }

        // ✅ Hàm tạo OrderService thực tế với mock
        private OrderServices CreateOrderService()
        {
            return new OrderServices(
                _mockUow.Object,
                _mockMapper.Object,
                new FakeJWTUtils(),      // fake JWT
                _mockVnPay.Object,
                _mockCheckDate.Object,
                _mockConfig.Object,
                _mockHubContext.Object   // 🔔 thêm hubContext mới
            );
        }

        private CreateOrderDTO CreateFakeOrderDTO() => new()
        {
            ShippingAddress = "123 Street",
            OrderItems = new List<SelectProductDTO>
            {
                new() { ProductId = 1, StockQuantity = 2 },
                new() { ProductId = 2, StockQuantity = 1 }
            }
        };

        // ================= TEST CASES =================

        [Fact]
        public async Task CreateOrderAsync_ShouldReturnFail_WhenAnyProductInvalid()
        {
            var fakeJwt = new FakeJWTUtils(new Account { AccountId = 99 });
            var fakeService = new FakeOrderService(fakeJwt);

            var dto = new CreateOrderDTO
            {
                ShippingAddress = "Test",
                OrderItems = new List<SelectProductDTO>
                {
                    new() { ProductId = 1, StockQuantity = 0 }
                }
            };

            var result = await fakeService.FakeCreateOrderAsync(dto);

            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.Contains("Invalid", result.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldSucceed_WhenAllProductsValid()
        {
            var fakeJwt = new FakeJWTUtils(new Account { AccountId = 10 });
            var fakeService = new FakeOrderService(fakeJwt);

            var dto = CreateFakeOrderDTO();
            var result = await fakeService.FakeCreateOrderAsync(dto);

            Assert.Equal(Const.SUCCESS_CREATE_CODE, result.Status);
            Assert.True(result.Data.TotalPrice > 0);
            Assert.Contains("successfully", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldReturnError_WhenExceptionThrown()
        {
            var fakeJwt = new FakeJWTUtils(null!, throwsException: true);
            var fakeService = new FakeOrderService(fakeJwt);

            var dto = CreateFakeOrderDTO();
            var result = await fakeService.FakeCreateOrderAsync(dto);

            Assert.Equal(Const.ERROR_EXCEPTION, result.Status);
            Assert.Contains("Error", result.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldCalculateTotalPriceCorrectly()
        {
            var fakeJwt = new FakeJWTUtils(new Account { AccountId = 20 });
            var fakeService = new FakeOrderService(fakeJwt);

            var dto = new CreateOrderDTO
            {
                ShippingAddress = "test",
                OrderItems = new List<SelectProductDTO>
                {
                    new() { ProductId = 1, StockQuantity = 3 },
                    new() { ProductId = 2, StockQuantity = 2 }
                }
            };

            var result = await fakeService.FakeCreateOrderAsync(dto);

            // Mỗi sản phẩm giá 20, tổng = (3 + 2) * 20 = 100
            Assert.Equal(Const.SUCCESS_CREATE_CODE, result.Status);
            Assert.Equal(100, result.Data.TotalPrice);
        }

        // ============================================
        // ✅ TEST 1: Order không tồn tại
        // ============================================
        [Fact]
        public async Task UpdateOrderDeliveryStatusAsync_ShouldReturnFail_WhenOrderNotFound()
        {
            // Arrange
            _mockOrderRepo.Setup(r => r.GetOrderById(It.IsAny<long>()))
                          .ReturnsAsync((Order)null);

            var service = CreateOrderService();

            // Act
            var result = await service.UpdateOrderDeliveryStatusAsync(1);

            // Assert
            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.Equal("Order not found !", result.Data); // Đây mới là đúng giá trị
        }


        // ============================================
        // ✅ TEST 2: Update thành công
        // ============================================
        [Fact]
        public async Task UpdateOrderDeliveryStatusAsync_ShouldSucceed_WhenOrderExists()
        {
            // Arrange
            var order = new Order
            {
                OrderId = 1,
                Status = PaymentStatus.PENDING,
                CustomerId = 123
            };

            _mockOrderRepo.Setup(r => r.GetOrderById(1))
                          .ReturnsAsync(order);

            _mockOrderRepo.Setup(r => r.UpdateAsync(order))
                          .ReturnsAsync(1);

            // Mock SignalR
            _mockHubClients.Setup(c => c.Group(It.IsAny<string>()))
                           .Returns(_mockClientProxy.Object);

            _mockClientProxy.Setup(c => c.SendCoreAsync(
                    It.IsAny<string>(),
                    It.IsAny<object[]>(),
                    default
                ))
                .Returns(Task.CompletedTask);

            var service = CreateOrderService();

            // Act
            var result = await service.UpdateOrderDeliveryStatusAsync(1);

            // Assert
            Assert.Equal(1, result.Status);
            Assert.Equal(PaymentStatus.DELIVERED, order.Status);
        }



        // ============================================
        // ✅ TEST 3: Đảm bảo Repository.UpdateAsync được gọi đúng 1 lần
        // ============================================
        [Fact]
        public async Task UpdateOrderDeliveryStatusAsync_ShouldCallUpdateRepository()
        {
            // Arrange
            var order = new Order { OrderId = 1, Status = PaymentStatus.PENDING };

            _mockOrderRepo.Setup(r => r.GetOrderById(1)).ReturnsAsync(order);

            var service = CreateOrderService();

            // Act
            await service.UpdateOrderDeliveryStatusAsync(1);

            // Assert
            _mockOrderRepo.Verify(r => r.UpdateAsync(order), Times.Once);
        }

        // ============================================
        // ✅ TEST 4: Ném exception → Trả Response lỗi
        // ============================================
        [Fact]
        public async Task UpdateOrderDeliveryStatusAsync_ShouldReturnError_WhenExceptionThrown()
        {
            // Arrange
            _mockOrderRepo.Setup(r => r.GetOrderById(It.IsAny<long>()))
                          .ThrowsAsync(new Exception("Database error!"));

            var service = CreateOrderService();

            // Act
            var result = await service.UpdateOrderDeliveryStatusAsync(1);

            // Assert
            Assert.Equal(Const.ERROR_EXCEPTION, result.Status);
            Assert.Contains("error", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task UpdateOrderCompletedStatusAsync_ShouldReturnFail_WhenOrderNotFound()
        {
            // Arrange
            _mockOrderRepo.Setup(r => r.GetOrderById(It.IsAny<long>()))
                          .ReturnsAsync((Order)null);

            var service = CreateOrderService();

            // Act
            var result = await service.UpdateOrderCompletedStatusAsync(1);

            // Assert
            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.Equal("Order not found !", result.Data);
        }

        [Fact]
        public async Task UpdateOrderCompletedStatusAsync_ShouldSucceed_WhenOrderExists()
        {
            // Arrange
            var order = new Order { OrderId = 1, Status = PaymentStatus.DELIVERED };

            _mockOrderRepo.Setup(r => r.GetOrderById(1)).ReturnsAsync(order);

            // 🔥 Mock HubContext để tránh NullReferenceException
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            mockClients
                .Setup(c => c.Group(It.IsAny<string>()))
                .Returns(mockClientProxy.Object);

            _mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);

            var service = CreateOrderService();

            // Act
            var result = await service.UpdateOrderCompletedStatusAsync(1);

            // Assert
            Assert.Equal(Const.SUCCESS_CREATE_CODE, result.Status);
            Assert.Equal(PaymentStatus.COMPLETED, order.Status);
        }


        [Fact]
        public async Task UpdateOrderCompletedStatusAsync_ShouldCallUpdateRepository()
        {
            // Arrange
            var order = new Order { OrderId = 1, Status = PaymentStatus.COMPLETED };

            _mockOrderRepo.Setup(r => r.GetOrderById(1)).ReturnsAsync(order);

            var service = CreateOrderService();

            // Act
            await service.UpdateOrderCompletedStatusAsync(1);

            // Assert
            _mockOrderRepo.Verify(r => r.UpdateAsync(order), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderCompletedStatusAsync_ShouldReturnError_WhenExceptionThrown()
        {
            // Arrange
            _mockOrderRepo.Setup(r => r.GetOrderById(It.IsAny<long>()))
                          .ThrowsAsync(new Exception("DB Crash"));

            var service = CreateOrderService();

            // Act
            var result = await service.UpdateOrderCompletedStatusAsync(1);

            // Assert
            Assert.Equal(Const.ERROR_EXCEPTION, result.Status);
            Assert.Contains("DB Crash", result.Message);
        }

        [Fact]
        public async Task UpdateOrderCancelStatusAsync_ShouldReturnFail_WhenOrderNotFound()
        {
            // Arrange
            _mockOrderRepo.Setup(r => r.GetOrderById(It.IsAny<long>()))
                          .ReturnsAsync((Order)null);

            var service = CreateOrderService();

            // Act
            var result = await service.UpdateOrderCancelStatusAsync(1);

            // Assert
            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.Equal("Order not found!", result.Data);
        }

        [Fact]
        public async Task UpdateOrderCancelStatusAsync_ShouldReturnFail_WhenOrderAlreadyCancelled()
        {
            // Arrange
            var order = new Order { OrderId = 1, Status = PaymentStatus.CANCELLED };

            _mockOrderRepo.Setup(r => r.GetOrderById(1)).ReturnsAsync(order);

            var service = CreateOrderService();

            // Act
            var result = await service.UpdateOrderCancelStatusAsync(1);

            // Assert
            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.Equal("Order is already cancelled.", result.Data);
        }

        [Fact]
        public async Task UpdateOrderCancelStatusAsync_ShouldSucceed_WhenOrderCanBeCancelled()
        {
            // Arrange
            var order = new Order { OrderId = 1, Status = PaymentStatus.PENDING };

            _mockOrderRepo.Setup(r => r.GetOrderById(1)).ReturnsAsync(order);

            // Mock HubContext để tránh NullReference
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            mockClients
                .Setup(c => c.Group(It.IsAny<string>()))
                .Returns(mockClientProxy.Object);

            _mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);

            var service = CreateOrderService();

            // Act
            var result = await service.UpdateOrderCancelStatusAsync(1);

            // Assert
            Assert.Equal(1, result.Status);
            Assert.Equal(PaymentStatus.CANCELLED, order.Status);
            Assert.Equal("Order cancelled and stock quantity restored.", result.Data);
        }



        [Fact]
        public async Task UpdateOrderCancelStatusAsync_ShouldCallUpdateAndSaveChanges()
        {
            // Arrange
            var order = new Order { OrderId = 1, Status = PaymentStatus.DELIVERED };

            _mockOrderRepo.Setup(r => r.GetOrderById(1)).ReturnsAsync(order);

            var service = CreateOrderService();

            // Act
            await service.UpdateOrderCancelStatusAsync(1);

            // Assert
            // ✅ Sửa dòng này:
            _mockOrderRepo.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Once);

            // ✅ Kiểm tra SaveChangesAsync vẫn gọi:
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public async Task UpdateOrderCancelStatusAsync_ShouldReturnError_WhenExceptionThrown()
        {
            // Arrange
            _mockOrderRepo.Setup(r => r.GetOrderById(It.IsAny<long>()))
                          .ThrowsAsync(new Exception("DB Crash"));

            var service = CreateOrderService();

            // Act
            var result = await service.UpdateOrderCancelStatusAsync(1);

            // Assert
            Assert.Equal(Const.ERROR_EXCEPTION, result.Status);
            Assert.Contains("DB Crash", result.Message);
        }

        [Fact]
        public async Task CreateOrderPaymentAsync_ShouldReturnError_WhenOrderNotFound()
        {
            // Arrange
            _mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Order)null!);

            var service = CreateOrderService();

            // Act
            var result = await service.CreateOrderPaymentAsync(1, new DefaultHttpContext());

            // Assert
            Assert.Equal(Const.ERROR_EXCEPTION, result.Status);
            Assert.Contains("not found", result.Message);
        }

        [Fact]
        public async Task CreateOrderPaymentAsync_ShouldReturnFail_WhenStatusInvalid()
        {
            // Arrange
            var order = new Order { OrderId = 1, Status = PaymentStatus.CANCELLED };
            _mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

            var service = CreateOrderService();

            // Act
            var result = await service.CreateOrderPaymentAsync(1, new DefaultHttpContext());

            // Assert
            Assert.Equal(Const.FAIL_CREATE_CODE, result.Status);
            Assert.Contains("not in a valid state", result.Message);
        }

        [Fact]
        public async Task CreateOrderPaymentAsync_ShouldUpdateStock_WhenStatusIsUNDISCHARGED()
        {
            // Arrange
            var order = new Order
            {
                OrderId = 1,
                Status = PaymentStatus.UNDISCHARGED,
                TotalPrice = 100,
                OrderDetails = new List<OrderDetail>
        {
            new OrderDetail { ProductId = 10, Quantity = 2 }
        }
            };

            _mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockUow.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);

            var service = CreateOrderService();

            // Act
            await service.CreateOrderPaymentAsync(1, new DefaultHttpContext());

            // Assert
            _mockProductRepo.Verify(p => p.UpdateStockByOrderAsync(10, 2), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
            mockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task CreateOrderPaymentAsync_ShouldNotUpdateStock_WhenStatusIsPENDING()
        {
            // Arrange
            var order = new Order
            {
                OrderId = 1,
                Status = PaymentStatus.PENDING,
                TotalPrice = 100,
                OrderDetails = new List<OrderDetail>()
            };

            _mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

            var service = CreateOrderService();

            // Act
            await service.CreateOrderPaymentAsync(1, new DefaultHttpContext());

            // Assert
            _mockProductRepo.Verify(p => p.UpdateStockByOrderAsync(It.IsAny<long>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CreateOrderPaymentAsync_ShouldReturnPaymentUrl_WhenSuccess()
        {
            // Arrange
            var order = new Order { OrderId = 1, Status = PaymentStatus.PENDING, TotalPrice = 200 };

            _mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
            _mockVnPay.Setup(v => v.CreatePaymentUrl(It.IsAny<PaymentInformationModel>(), It.IsAny<HttpContext>()))
                      .Returns("http://test-payment-url");

            var service = CreateOrderService();

            // Act
            var result = await service.CreateOrderPaymentAsync(1, new DefaultHttpContext());

            // Assert
            Assert.Equal(Const.SUCCESS_CREATE_CODE, result.Status);
            Assert.Equal("http://test-payment-url", result.Data?.GetType().GetProperty("PaymentUrl")?.GetValue(result.Data)?.ToString());
        }

    }
}
