using Application.Commons;
using Application.Repositories;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Response.OrderResponse;

namespace Infrastructure.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Pagination<OrderResultDTO>> GetAllOrdersAsync(int pageIndex, int pageSize);
        Task<Pagination<OrderResultDTO>> GetOrdersByCustomerIdAsync(long customerId, int pageIndex, int pageSize);
        Task<OrderResultDTO> GetOrderByIdAsync(long orderId);
        Task<Order?> GetOrderById(long orderId);
    }
}
