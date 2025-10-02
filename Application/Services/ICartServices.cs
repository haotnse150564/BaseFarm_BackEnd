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
        Task<bool> AddToCart(Product products, int quantity);
        Task<CartResponse> RemoveFromCart();
        Task<CartResponse> UpdateCartItem(Product products, int quantity);
        Task<IEnumerable<CartResponse>> GetCartItems();
        Task<CartResponse> ClearCart();
    }
}
