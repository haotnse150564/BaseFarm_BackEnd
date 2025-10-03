using Application.Interfaces;
using Application.Utils;
using AutoMapper;
using Domain.Model;
using Infrastructure.ViewModel.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implement
{
    public class CartServices : ICartServices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        private readonly JWTUtils _jwtUtils;
        public CartServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper, JWTUtils jWTUtils)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
            _jwtUtils = jWTUtils;
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
    }
}
