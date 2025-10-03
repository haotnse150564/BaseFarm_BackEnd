using Domain.Model;
using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ICartServices
    {
        Task<bool> AddToCart(long productId , int quantity);
        Task<CartResponse> RemoveCartItem(long productId);
        Task<CartResponse> UpdateCartItem(long productId, int quantity);
        Task<CartResponse> GetCartItems();
        Task<CartResponse> ClearCart();
    }
}
