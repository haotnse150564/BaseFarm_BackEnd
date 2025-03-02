using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Request.OrderRequest;
using static Application.ViewModel.Response.OrderResponse;

namespace Application.Services
{
    public interface IOrderServices
    {
        Task<ResponseDTO> CreateOrderAsync(CreateOrderDTO request);
        Task<ResponseDTO> GetAllOrderAsync(int pageIndex, int pageSize);
    }
}
