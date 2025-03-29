using Application.Repositories;
using Domain.Model;

namespace Infrastructure.Repositories
{
    public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
    {
        Task<List<OrderDetail>> GetOrderDetailsByOrderId(long orderId);
    }
}
