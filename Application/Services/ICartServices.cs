using Domain.Model;
using Infrastructure.ViewModel.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Response.OrderResponse;

namespace Application.Services
{
    public interface ICartServices
    {
        Task<bool> AddToCart(long productId , int quantity);
        Task<CartResponse> RemoveCartItem(long productId);
        Task<CartResponse> UpdateCartItem(long productId, int quantity);
        Task<CartResponse> GetCartItems();
        Task<CartResponse> ClearCart();
        Task<ResponseDTO> PrepareOrderAsync(HttpContext context);
        Task<ResponseDTO> BuyAgainAsync(long orderId);
    }
}
