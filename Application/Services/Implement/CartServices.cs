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
        public async Task<bool> AddToCart(Product products, int quantity)
        {
            var result = false;
            var userId = _jwtUtils.GetCurrentUserAsync().Result.AccountId;
            if (userId == 0)
            {
                return false;
            }
            else
            {
                var cart = await _unitOfWork.cartRepository.GetCartByUserIdAsync(userId);
                var product = await _unitOfWork.productRepository.GetByIdAsync(products.ProductId);
                if (cart.CartItems.IsNullOrEmpty() || (cart.CartItems.FirstOrDefault(x => x.ProductId == products.ProductId)).Equals(null))
                {
                    CartItem cartItem = new CartItem
                    {
                        ProductId = products.ProductId,
                        Quantity = quantity,
                        PriceQuantity = products.Price,
                        CartId = cart.CartId
                    };
                    await _unitOfWork.cartItemRepository.AddAsync(cartItem);
                    result = true;
                }
                else
                { 
                    cart.CartItems.FirstOrDefault(x => x.ProductId == products.ProductId)!.Quantity += quantity;
                    return true;
                }
            }
            return false;
        }

        public Task<CartResponse> RemoveFromCart()
        {
            throw new NotImplementedException();
        }

        public Task<CartResponse> UpdateCartItem(Product products, int quantity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CartResponse>> GetCartItems()
        {
            throw new NotImplementedException();
        }

        public Task<CartResponse> ClearCart()
        {
            throw new NotImplementedException();
        }
    }
}
