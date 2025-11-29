using Application.Interfaces;
using Application.Utils;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure.ViewModel.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Request.OrderRequest;
using static Application.ViewModel.Response.OrderResponse;

namespace Application.Services.Implement
{
    public class CartServices : ICartServices
    {
        protected readonly IUnitOfWorks _unitOfWork;
        protected readonly ICurrentTime _currentTime;
        protected readonly IConfiguration configuration;
        protected readonly IMapper _mapper;
        protected readonly JWTUtils _jwtUtils;
        protected readonly IOrderServices _orderServices;
        public CartServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper, JWTUtils jWTUtils, IOrderServices orderServices)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
            _jwtUtils = jWTUtils;
            _orderServices = orderServices;
        }
        public async Task<bool> AddToCart(long productId, int quantity)
        {
            var userId = await _jwtUtils.GetCurrentUserAsync();
            if (userId.AccountId == 0)
            {
                return false;
            }
            else
            {
                if (await _unitOfWork.cartRepository.GetCartByUserIdAsync(userId.AccountId) == null)
                {
                    Cart newCart = new Cart
                    {
                        PaymentStatus = 0,
                        CustomerId = userId.AccountId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.cartRepository.AddAsync(newCart);
                    await _unitOfWork.SaveChangesAsync();
                }
                var cart = await _unitOfWork.cartRepository.GetCartByUserIdAsync(userId.AccountId);
                var product = await _unitOfWork.productRepository.GetByIdAsync(productId);
                var isExist = cart.CartItems.Any(x => x.ProductId == productId);
                if (/*cart.CartItems.IsNullOrEmpty() || */!isExist)
                {
                    CartItem cartItem = new CartItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        PriceQuantity = (product.Price * quantity),
                        CartId = cart.CartId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow

                    };
                    await _unitOfWork.cartItemRepository.AddAsync(cartItem);
                    if (await _unitOfWork.SaveChangesAsync() < 0) return false;
                    return true;
                }
                else
                {
                    var item = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
                    item.Quantity += quantity;
                    await _unitOfWork.cartItemRepository.UpdateAsync(item);
                    if (await _unitOfWork.SaveChangesAsync() < 0) return false;
                    return true;
                }
            }
        }

        public async Task<CartResponse> RemoveCartItem(long productId)
        {
            var userId = await _jwtUtils.GetCurrentUserAsync();
            if (userId.AccountId == 0)
            {
                return null;
            }
            else
            {
                var cart = await _unitOfWork.cartRepository.GetCartByUserIdAsync(userId.AccountId);
                var item = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
                if (item == null) return null;
                await _unitOfWork.cartItemRepository.DeleteAsync(item);
                await _unitOfWork.SaveChangesAsync();
                var cartResponse = _mapper.Map<CartResponse>(cart);
                return cartResponse;
            }

        }

        public async Task<CartResponse> UpdateCartItem(long productId, int quantity)
        {
            var userId = await _jwtUtils.GetCurrentUserAsync();

            if (userId.AccountId == 0)
            {
                return null;
            }
            else
            {
                var cart = await _unitOfWork.cartRepository.GetCartByUserIdAsync(userId.AccountId);
                var product = await _unitOfWork.productRepository.GetByIdAsync(productId);
                var item = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
                if (item == null) return null;
                item.Quantity = quantity;
                item.PriceQuantity = (product.Price * quantity);

                await _unitOfWork.cartItemRepository.UpdateAsync(item);
                await _unitOfWork.SaveChangesAsync();
                var cartResponse = _mapper.Map<CartResponse>(cart);
                return cartResponse;
            }
        }

        public async Task<ResponseDTO> PrepareOrderAsync(HttpContext context)
        {
            // Lấy user hiện tại
            var user = await _jwtUtils.GetCurrentUserAsync();
            if (user == null || user.AccountId == 0)
            {
                return new ResponseDTO ( 401, "Unauthorized user." );
            }

            // Lấy giỏ hàng hiện tại
            var cart = await _unitOfWork.cartRepository.GetCartByUserIdAsync(user.AccountId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                return new ResponseDTO (400, "Cart is empty.");
            }

            // Tạo CreateOrderDTO từ cart
            var createOrderDTO = new CreateOrderDTO
            {
                OrderItems = cart.CartItems.Select(item => new SelectProductDTO
                {
                    ProductId = item.ProductId,
                    StockQuantity = item.Quantity
                }).ToList(),
                ShippingAddress = user.AccountProfile.Address 
            };          

            return new ResponseDTO(200, "Order Success.", createOrderDTO); 
        }
        public async Task<CartResponse> GetCartItems()
        
        {
            var userId = await _jwtUtils.GetCurrentUserAsync();
            var item = await _unitOfWork.cartRepository.GetCartByUserIdAsync(userId.AccountId);
            if (item == null) return null;
            var result = _mapper.Map<CartResponse>(item);
            return result;
        }

        public async Task<CartResponse> ClearCart()
        {
            var userId = await _jwtUtils.GetCurrentUserAsync();
            if (userId.AccountId == 0)
            {
                return null;
            }
            else
            {
                var cart = await _unitOfWork.cartRepository.GetCartByUserIdAsync(userId.AccountId);
                foreach (var item in cart.CartItems)
                {
                    await _unitOfWork.cartItemRepository.DeleteAsync(item);
                }
                await _unitOfWork.SaveChangesAsync();
                var cartResponse = _mapper.Map<CartResponse>(cart);
                return cartResponse;
            }
        }

        public async Task<ResponseDTO> BuyAgainAsync(long orderId)
        {
            try
            {
                // Lấy thông tin order cũ
                var order = await _unitOfWork.orderRepository.GetOrderById(orderId);
                if (order == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Order not found.");
                }

                // check status
                if (order.Status != PaymentStatus.COMPLETED && order.Status != PaymentStatus.CANCELLED)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Only completed or cancelled orders can be purchased again.");
                }

                // Lấy user hiện tại
                var user = await _jwtUtils.GetCurrentUserAsync();
                if (user.AccountId == 0)
                {
                    return new ResponseDTO(Const.WARNING_NO_DATA_CODE, "User not authenticated.");
                }

                // Lấy danh sách sản phẩm trong order
                var orderItems = order.OrderDetails;
                if (orderItems == null || !orderItems.Any())
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Order has no items.");
                }

                // Duyệt từng sản phẩm và add vào cart
                foreach (var item in orderItems)
                {
                    var result = await AddToCart(item.ProductId, item.Quantity ?? 0);

                    if (!result)
                    {
                        return new ResponseDTO(Const.FAIL_CREATE_CODE,
                            $"Failed to add product {item.ProductId} to cart.");
                    }
                }

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Added all items to cart successfully.");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

    }
}
