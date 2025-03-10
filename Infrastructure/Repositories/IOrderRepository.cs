using Application.Commons;
using Application.Repositories;
using Domain.Model;
using static Application.ViewModel.Response.OrderResponse;

namespace Infrastructure.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Pagination<OrderResultDTO>> GetAllOrdersAsync(int pageIndex, int pageSize);
        Task<Pagination<OrderResultDTO>> GetOrdersByCustomerIdAsync(long customerId, int pageIndex, int pageSize);
        Task<OrderResultDTO> GetOrderByIdAsync(long orderId);
        Task<Order?> GetOrderById(long orderId);
        Task<Pagination<OrderResultDTO>> GetOrdersByCustomerNameAsync(string customerName, int pageIndex, int pageSize);
    }
}
