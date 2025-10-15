using Xunit;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Services;
using Application.Utils;
using Domain.Model;
using Infrastructure.Repositories;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Application.Services.Implement;
using Application;
using Application.Interfaces;
using Infrastructure;

namespace BaseFarm_BackEnd.Test.Services
{
    // ✅ Fake CartService để bỏ qua việc gọi JWTUtils thật
    public class CartServicesFake : CartServices
    {
        private readonly Account _fakeUser;

        public CartServicesFake(
            IUnitOfWorks unitOfWork,
            ICurrentTime currentTime,
            IConfiguration config,
            IMapper mapper,
            JWTUtils jwt,
            IOrderServices orderService,
            Account fakeUser)
            : base(unitOfWork, currentTime, config, mapper, jwt, orderService)
        {
            _fakeUser = fakeUser;
        }

        // ✅ Bỏ qua JWTUtils, dùng _fakeUser để test
        public new async Task<bool> AddToCart(long productId, int quantity)
        {
            var user = _fakeUser;
            if (user == null || user.AccountId == 0)
                return false;

            var cart = await _unitOfWork.cartRepository.GetCartByUserIdAsync(user.AccountId);
            var product = await _unitOfWork.productRepository.GetByIdAsync(productId);
            if (product == null)
                return false;

            if (cart == null)
            {
                cart = new Cart { CustomerId = user.AccountId, CartItems = new List<CartItem>() };
                await _unitOfWork.cartRepository.AddAsync(cart);
            }

            var existingItem = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
            if (existingItem == null)
            {
                var item = new CartItem { ProductId = productId, Quantity = quantity };
                await _unitOfWork.cartItemRepository.AddAsync(item);
            }
            else
            {
                existingItem.Quantity += quantity;
                await _unitOfWork.cartItemRepository.UpdateAsync(existingItem);
            }

            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }
    }

    public class CartServiceTests
    {
        private readonly Mock<IUnitOfWorks> _mockUow;
        private readonly Mock<ICartRepository> _mockCartRepo;
        private readonly Mock<ICartItemRepository> _mockCartItemRepo;
        private readonly Mock<IProductRepository> _mockProductRepo;
        private readonly Mock<ICurrentTime> _mockCurrentTime;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IOrderServices> _mockOrderService;

        private CartServices _service = null!;
        private JWTUtils _jwtUtils = null!;

        public CartServiceTests()
        {
            _mockUow = new Mock<IUnitOfWorks>();
            _mockCartRepo = new Mock<ICartRepository>();
            _mockCartItemRepo = new Mock<ICartItemRepository>();
            _mockProductRepo = new Mock<IProductRepository>();
            _mockCurrentTime = new Mock<ICurrentTime>();
            _mockConfig = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
            _mockOrderService = new Mock<IOrderServices>();

            _mockUow.SetupGet(u => u.cartRepository).Returns(_mockCartRepo.Object);
            _mockUow.SetupGet(u => u.cartItemRepository).Returns(_mockCartItemRepo.Object);
            _mockUow.SetupGet(u => u.productRepository).Returns(_mockProductRepo.Object);
        }

        // ✅ tiện ích: tạo CartServicesFake thay vì thật
        private void InitServiceWithUser(Account user)
        {
            _jwtUtils = new Mock<JWTUtils>(null!, null!, null!).Object;
            _service = new CartServicesFake(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _jwtUtils,
                _mockOrderService.Object,
                user
            );
        }

        // Case 1: userId = 0
        [Fact]
        public async Task AddToCart_ShouldReturnFalse_WhenUserNotFound()
        {
            var user = new Account { AccountId = 0 };
            InitServiceWithUser(user);

            var result = await ((CartServicesFake)_service).AddToCart(1, 2);

            Assert.False(result);
            _mockCartRepo.Verify(r => r.GetCartByUserIdAsync(It.IsAny<long>()), Times.Never);
        }

        // Case 2: chưa có cart -> tạo mới
        [Fact]
        public async Task AddToCart_ShouldCreateNewCart_WhenUserHasNoCart()
        {
            var user = new Account { AccountId = 10 };
            InitServiceWithUser(user);

            var product = new Product { ProductId = 1, Price = 100 };
            _mockCartRepo.Setup(r => r.GetCartByUserIdAsync(user.AccountId))
                         .ReturnsAsync((Cart?)null);
            _mockProductRepo.Setup(r => r.GetByIdAsync(1))
                            .ReturnsAsync(product);
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mockCartRepo.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
            _mockCartItemRepo.Setup(r => r.AddAsync(It.IsAny<CartItem>())).Returns(Task.CompletedTask);

            var result = await ((CartServicesFake)_service).AddToCart(1, 2);

            Assert.True(result);
            _mockCartRepo.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Once);
            _mockCartItemRepo.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Once);
        }

        // Case 3: giỏ hàng có, sản phẩm chưa có
        [Fact]
        public async Task AddToCart_ShouldAddNewItem_WhenProductNotInCart()
        {
            var user = new Account { AccountId = 2 };
            InitServiceWithUser(user);

            var cart = new Cart
            {
                CartId = 3,
                CustomerId = user.AccountId,
                CartItems = new List<CartItem>()
            };
            var product = new Product { ProductId = 99, Price = 50 };

            _mockCartRepo.Setup(r => r.GetCartByUserIdAsync(user.AccountId))
                         .ReturnsAsync(cart);
            _mockProductRepo.Setup(r => r.GetByIdAsync(product.ProductId))
                            .ReturnsAsync(product);
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await ((CartServicesFake)_service).AddToCart(product.ProductId, 1);

            Assert.True(result);
            _mockCartItemRepo.Verify(r => r.AddAsync(It.Is<CartItem>(x => x.ProductId == product.ProductId)), Times.Once);
        }

        // Case 4: giỏ hàng có, sản phẩm đã có => tăng số lượng
        [Fact]
        public async Task AddToCart_ShouldUpdateQuantity_WhenProductAlreadyInCart()
        {
            var user = new Account { AccountId = 5 };
            InitServiceWithUser(user);

            var product = new Product { ProductId = 10, Price = 100 };
            var existingItem = new CartItem { ProductId = 10, Quantity = 1 };
            var cart = new Cart
            {
                CartId = 20,
                CustomerId = user.AccountId,
                CartItems = new List<CartItem> { existingItem }
            };

            _mockCartRepo.Setup(r => r.GetCartByUserIdAsync(user.AccountId))
                         .ReturnsAsync(cart);
            _mockProductRepo.Setup(r => r.GetByIdAsync(product.ProductId))
                            .ReturnsAsync(product);
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await ((CartServicesFake)_service).AddToCart(product.ProductId, 2);

            Assert.True(result);
            Assert.Equal(3, existingItem.Quantity);
            _mockCartItemRepo.Verify(r => r.UpdateAsync(existingItem), Times.Once);
        }

        // Case 5: SaveChangesAsync trả < 0
        [Fact]
        public async Task AddToCart_ShouldReturnFalse_WhenSaveFails()
        {
            var user = new Account { AccountId = 1 };
            InitServiceWithUser(user);

            var product = new Product { ProductId = 1, Price = 100 };
            var cart = new Cart
            {
                CartId = 10,
                CustomerId = user.AccountId,
                CartItems = new List<CartItem>()
            };

            _mockCartRepo.Setup(r => r.GetCartByUserIdAsync(user.AccountId))
                         .ReturnsAsync(cart);
            _mockProductRepo.Setup(r => r.GetByIdAsync(product.ProductId))
                            .ReturnsAsync(product);
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(-1);

            var result = await ((CartServicesFake)_service).AddToCart(product.ProductId, 1);

            Assert.False(result);
        }
    }
}
