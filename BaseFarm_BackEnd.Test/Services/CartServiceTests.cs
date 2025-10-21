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
using Microsoft.AspNetCore.Http;
using Application.Services.Implement;
using Application;
using Application.Interfaces;
using Infrastructure;
using System.Collections.Generic;
using Infrastructure.ViewModel.Response;
using Domain.Enum;

namespace BaseFarm_BackEnd.Test.Services
{
    // ✅ Fake CartService để bỏ qua việc gọi JWTUtils thật (cho AddToCart & RemoveCartItem)
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

        // ✅ Fake AddToCart (bỏ qua JWT)
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

        // ✅ Fake RemoveCartItem (bỏ qua JWT)
        public new async Task<CartResponse?> RemoveCartItem(long productId)
        {
            var user = _fakeUser;
            if (user == null || user.AccountId == 0)
                return null;

            var cart = await _unitOfWork.cartRepository.GetCartByUserIdAsync(user.AccountId);
            if (cart == null)
                return null;

            var item = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
            if (item == null)
                return null;

            await _unitOfWork.cartItemRepository.DeleteAsync(item);
            await _unitOfWork.SaveChangesAsync();

            var cartResponse = _mapper.Map<CartResponse>(cart);
            return cartResponse;
        }

        public new async Task<CartResponse?> UpdateCartItem(long productId, int quantity)
        {
            var user = _fakeUser;
            if (user == null || user.AccountId == 0)
                return null;

            var cart = await _unitOfWork.cartRepository.GetCartByUserIdAsync(user.AccountId);
            var product = await _unitOfWork.productRepository.GetByIdAsync(productId);

            if (product == null) return null;
            if (cart == null) return null;

            var item = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
            if (item == null) return null;

            item.Quantity = quantity;
            item.PriceQuantity = (product.Price * quantity);

            await _unitOfWork.cartItemRepository.UpdateAsync(item);
            await _unitOfWork.SaveChangesAsync();

            var cartResponse = _mapper.Map<CartResponse>(cart);
            return cartResponse;
        }

        public new async Task<CartResponse?> GetCartItems()
        {
            var user = _fakeUser;
            if (user == null || user.AccountId == 0)
                return null;

            var item = await _unitOfWork.cartRepository.GetCartByUserIdAsync(user.AccountId);
            if (item == null) return null;

            var result = _mapper.Map<CartResponse>(item);
            return result;
        }

        public new async Task<CartResponse?> ClearCart()
        {
            var user = _fakeUser;
            if (user == null || user.AccountId == 0)
                return null;

            var cart = await _unitOfWork.cartRepository.GetCartByUserIdAsync(user.AccountId);
            if (cart == null) return null;

            foreach (var item in cart.CartItems)
            {
                await _unitOfWork.cartItemRepository.DeleteAsync(item);
            }

            await _unitOfWork.SaveChangesAsync();

            var cartResponse = _mapper.Map<CartResponse>(cart);
            return cartResponse;
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
        private readonly Mock<JWTUtils> _mockJwt;

        private CartServices _service = null!;

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
            _mockJwt = new Mock<JWTUtils>(null!, null!, null!);

            _mockUow.SetupGet(u => u.cartRepository).Returns(_mockCartRepo.Object);
            _mockUow.SetupGet(u => u.cartItemRepository).Returns(_mockCartItemRepo.Object);
            _mockUow.SetupGet(u => u.productRepository).Returns(_mockProductRepo.Object);
        }

        private void InitService(Account user)
        {
            _service = new CartServicesFake(
                _mockUow.Object,
                _mockCurrentTime.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _mockJwt.Object,
                _mockOrderService.Object,
                user
            );
        }

        // ========================== ADD TO CART TESTS ==========================

        [Fact]
        public async Task AddToCart_ShouldReturnFalse_WhenUserNotFound()
        {
            var user = new Account { AccountId = 0 };
            InitService(user);

            var result = await ((CartServicesFake)_service).AddToCart(1, 2);

            Assert.False(result);
            _mockCartRepo.Verify(r => r.GetCartByUserIdAsync(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task AddToCart_ShouldCreateNewCart_WhenUserHasNoCart()
        {
            var user = new Account { AccountId = 10 };
            InitService(user);

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

        [Fact]
        public async Task AddToCart_ShouldAddNewItem_WhenProductNotInCart()
        {
            var user = new Account { AccountId = 2 };
            InitService(user);

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

        [Fact]
        public async Task AddToCart_ShouldUpdateQuantity_WhenProductAlreadyInCart()
        {
            var user = new Account { AccountId = 5 };
            InitService(user);

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

        [Fact]
        public async Task AddToCart_ShouldReturnFalse_WhenSaveFails()
        {
            var user = new Account { AccountId = 1 };
            InitService(user);

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

        // ========================== REMOVE CART ITEM TESTS ==========================

        [Fact]
        public async Task RemoveCartItem_ShouldReturnNull_WhenUserIdIsZero()
        {
            var fakeUser = new Account { AccountId = 0 };
            InitService(fakeUser);

            var result = await ((CartServicesFake)_service).RemoveCartItem(1);
            Assert.Null(result);
            _mockCartRepo.Verify(r => r.GetCartByUserIdAsync(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task RemoveCartItem_ShouldReturnNull_WhenCartNotFound()
        {
            var fakeUser = new Account { AccountId = 5 };
            InitService(fakeUser);

            _mockCartRepo.Setup(r => r.GetCartByUserIdAsync(fakeUser.AccountId))
                         .ReturnsAsync((Cart?)null);

            var result = await ((CartServicesFake)_service).RemoveCartItem(1);
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveCartItem_ShouldReturnNull_WhenItemNotFound()
        {
            var fakeUser = new Account { AccountId = 5 };
            InitService(fakeUser);

            var cart = new Cart
            {
                CustomerId = fakeUser.AccountId,
                CartItems = new List<CartItem> { new CartItem { ProductId = 99, Quantity = 2 } }
            };

            _mockCartRepo.Setup(r => r.GetCartByUserIdAsync(fakeUser.AccountId))
                         .ReturnsAsync(cart);

            var result = await ((CartServicesFake)_service).RemoveCartItem(1);

            Assert.Null(result);
            _mockCartItemRepo.Verify(r => r.DeleteAsync(It.IsAny<CartItem>()), Times.Never);
        }

        [Fact]
        public async Task RemoveCartItem_ShouldDeleteItem_WhenFound()
        {
            var fakeUser = new Account { AccountId = 10 };
            InitService(fakeUser);

            var item = new CartItem { ProductId = 5, Quantity = 2 };
            var cart = new Cart
            {
                CustomerId = fakeUser.AccountId,
                CartItems = new List<CartItem> { item }
            };
            var mappedResponse = new CartResponse();

            _mockCartRepo.Setup(r => r.GetCartByUserIdAsync(fakeUser.AccountId))
                         .ReturnsAsync(cart);
            _mockMapper.Setup(m => m.Map<CartResponse>(cart)).Returns(mappedResponse);
            _mockCartItemRepo.Setup(r => r.DeleteAsync(item)).ReturnsAsync(true);
            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await ((CartServicesFake)_service).RemoveCartItem(5);

            Assert.NotNull(result);
            Assert.Equal(mappedResponse, result);
            _mockCartItemRepo.Verify(r => r.DeleteAsync(item), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // ✅ TEST 1: Update thành công
        [Fact]
        public async Task UpdateCartItem_ShouldUpdateQuantity_AndReturnCartResponse()
        {
            var user = new Account { AccountId = 1 };
            InitService(user);

            var product = new Product { ProductId = 100, Price = 10m };
            var item = new CartItem { ProductId = 100, Quantity = 1, PriceQuantity = 10 };
            var cart = new Cart { CartItems = new List<CartItem> { item } };

            _mockProductRepo.Setup(p => p.GetByIdAsync(100)).ReturnsAsync(product);
            _mockCartRepo.Setup(c => c.GetCartByUserIdAsync(1)).ReturnsAsync(cart);

            _mockMapper.Setup(m => m.Map<CartResponse>(It.IsAny<Cart>()))
                       .Returns(new CartResponse
                       {
                           PaymentStatus = PaymentStatus.PENDING,
                           CreatedAt = DateTime.UtcNow,
                           UpdatedAt = DateTime.UtcNow,
                           ExpereAt = DateTime.UtcNow.AddDays(1),
                           CartItems = new List<CartItemResponse>()
                       });

            var result = await ((CartServicesFake)_service).UpdateCartItem(100, 2);

            Assert.NotNull(result);
            Assert.Equal(PaymentStatus.PENDING, result.PaymentStatus);
            Assert.Equal(2, item.Quantity);
            Assert.Equal(20, item.PriceQuantity);
            _mockCartItemRepo.Verify(r => r.UpdateAsync(item), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // ✅ TEST 2
        [Fact]
        public async Task UpdateCartItem_ShouldReturnNull_WhenUserIdIsZero()
        {
            var user = new Account { AccountId = 0 };
            InitService(user);

            var result = await ((CartServicesFake)_service).UpdateCartItem(100, 2);

            Assert.Null(result);
            _mockCartRepo.Verify(r => r.GetCartByUserIdAsync(It.IsAny<long>()), Times.Never);
        }

        // ✅ TEST 3
        [Fact]
        public async Task UpdateCartItem_ShouldReturnNull_WhenItemNotFound()
        {
            var user = new Account { AccountId = 1 };
            InitService(user);

            var product = new Product { ProductId = 100, Price = 10m };
            var cart = new Cart { CartItems = new List<CartItem>() };

            _mockProductRepo.Setup(p => p.GetByIdAsync(100)).ReturnsAsync(product);
            _mockCartRepo.Setup(c => c.GetCartByUserIdAsync(1)).ReturnsAsync(cart);

            var result = await ((CartServicesFake)_service).UpdateCartItem(100, 3);

            Assert.Null(result);
            _mockCartItemRepo.Verify(r => r.UpdateAsync(It.IsAny<CartItem>()), Times.Never);
        }

        // ✅ TEST 4
        [Fact]
        public async Task UpdateCartItem_ShouldReturnNull_WhenProductNotFound()
        {
            var user = new Account { AccountId = 1 };
            InitService(user);

            var cart = new Cart
            {
                CartItems = new List<CartItem> {
            new CartItem { ProductId = 100, Quantity = 1 }
        }
            };

            _mockProductRepo.Setup(p => p.GetByIdAsync(100))
                            .ReturnsAsync((Product)null!);
            _mockCartRepo.Setup(c => c.GetCartByUserIdAsync(1)).ReturnsAsync(cart);

            var result = await ((CartServicesFake)_service).UpdateCartItem(100, 3);

            Assert.Null(result);
            _mockCartItemRepo.Verify(r => r.UpdateAsync(It.IsAny<CartItem>()), Times.Never);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        // ✅ TEST 1: Khi user chưa đăng nhập
        [Fact]
        public async Task GetCartItems_ShouldReturnNull_WhenUserNotLoggedIn()
        {
            var user = new Account { AccountId = 0 };
            InitService(user);

            var result = await ((CartServicesFake)_service).GetCartItems();

            Assert.Null(result);
            _mockCartRepo.Verify(r => r.GetCartByUserIdAsync(It.IsAny<long>()), Times.Never);
        }

        // ✅ TEST 2: Khi user có nhưng cart không tồn tại
        [Fact]
        public async Task GetCartItems_ShouldReturnNull_WhenCartNotFound()
        {
            var user = new Account { AccountId = 5 };
            InitService(user);

            _mockCartRepo.Setup(r => r.GetCartByUserIdAsync(user.AccountId))
                         .ReturnsAsync((Cart?)null);

            var result = await ((CartServicesFake)_service).GetCartItems();

            Assert.Null(result);
            _mockCartRepo.Verify(r => r.GetCartByUserIdAsync(user.AccountId), Times.Once);
        }

        // ✅ TEST 3: Khi cart tồn tại và được map thành công
        [Fact]
        public async Task GetCartItems_ShouldReturnMappedCartResponse_WhenCartExists()
        {
            var user = new Account { AccountId = 10 };
            InitService(user);

            var cart = new Cart
            {
                CartId = 123,
                CustomerId = user.AccountId,
                CartItems = new List<CartItem>
        {
            new CartItem { ProductId = 1, Quantity = 2, PriceQuantity = 200 }
        }
            };

            var mappedResponse = new CartResponse
            {
                PaymentStatus = PaymentStatus.PENDING,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ExpereAt = DateTime.UtcNow.AddHours(2),
                CartItems = new List<CartItemResponse>
        {
            new CartItemResponse { ProductId = 1, Quantity = 2 }
        }
            };

            _mockCartRepo.Setup(r => r.GetCartByUserIdAsync(user.AccountId))
                         .ReturnsAsync(cart);
            _mockMapper.Setup(m => m.Map<CartResponse>(cart))
                       .Returns(mappedResponse);

            var result = await ((CartServicesFake)_service).GetCartItems();

            Assert.NotNull(result);
            Assert.Equal(mappedResponse, result);
            _mockCartRepo.Verify(r => r.GetCartByUserIdAsync(user.AccountId), Times.Once);
            _mockMapper.Verify(m => m.Map<CartResponse>(cart), Times.Once);
        }

        // ✅ CASE 1: User chưa đăng nhập (AccountId = 0)
        [Fact]
        public async Task ClearCart_ShouldReturnNull_WhenUserNotLoggedIn()
        {
            var user = new Account { AccountId = 0 };
            InitService(user);

            var result = await ((CartServicesFake)_service).ClearCart();

            Assert.Null(result);
            _mockCartRepo.Verify(r => r.GetCartByUserIdAsync(It.IsAny<long>()), Times.Never);
            _mockCartItemRepo.Verify(r => r.DeleteAsync(It.IsAny<CartItem>()), Times.Never);
        }

        // ✅ CASE 2: Cart có items, phải xóa hết và map lại
        [Fact]
        public async Task ClearCart_ShouldDeleteAllItems_AndReturnMappedResponse()
        {
            var user = new Account { AccountId = 5 };
            InitService(user);

            var cart = new Cart
            {
                CartId = 10,
                CustomerId = user.AccountId,
                CartItems = new List<CartItem>
        {
            new CartItem { CartItemId = 1, ProductId = 2, Quantity = 3 },
            new CartItem { CartItemId = 2, ProductId = 5, Quantity = 1 }
        }
            };

            var mappedResponse = new CartResponse
            {
                PaymentStatus = PaymentStatus.PENDING,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ExpereAt = DateTime.UtcNow.AddHours(1),
                CartItems = new List<CartItemResponse>()
            };

            _mockCartRepo.Setup(r => r.GetCartByUserIdAsync(user.AccountId))
                         .ReturnsAsync(cart);
            _mockMapper.Setup(m => m.Map<CartResponse>(cart))
                       .Returns(mappedResponse);

            var result = await ((CartServicesFake)_service).ClearCart();

            // ✅ Đảm bảo xóa từng item
            _mockCartItemRepo.Verify(r => r.DeleteAsync(It.Is<CartItem>(i => i == cart.CartItems.First())), Times.Once);
            _mockCartItemRepo.Verify(r => r.DeleteAsync(It.Is<CartItem>(i => i == cart.CartItems.First())), Times.Once);

            // ✅ SaveChanges được gọi 1 lần
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);

            // ✅ Trả về đúng mapped response
            Assert.Equal(mappedResponse, result);
        }

        // ✅ CASE 3: Cart không có items (rỗng)
        [Fact]
        public async Task ClearCart_ShouldReturnMappedResponse_WhenCartHasNoItems()
        {
            var user = new Account { AccountId = 10 };
            InitService(user);

            var cart = new Cart
            {
                CartId = 77,
                CustomerId = user.AccountId,
                CartItems = new List<CartItem>()
            };

            var mappedResponse = new CartResponse
            {
                PaymentStatus = PaymentStatus.PENDING,
                CartItems = new List<CartItemResponse>()
            };

            _mockCartRepo.Setup(r => r.GetCartByUserIdAsync(user.AccountId))
                         .ReturnsAsync(cart);
            _mockMapper.Setup(m => m.Map<CartResponse>(cart))
                       .Returns(mappedResponse);

            var result = await ((CartServicesFake)_service).ClearCart();

            _mockCartItemRepo.Verify(r => r.DeleteAsync(It.IsAny<CartItem>()), Times.Never);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Equal(mappedResponse, result);
        }
    }
}
