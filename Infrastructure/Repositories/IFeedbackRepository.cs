using Application.Repositories;
using Domain.Model;

namespace Infrastructure.Repositories
{
    public interface IFeedbackRepository : IGenericRepository<Feedback>
    {
        Task<int> CountAsync();
        Task<IEnumerable<Feedback>> GetPagedAsync(int pageIndex, int pageSize);
        Task<Feedback> GetByIdAsync(long id);
    }
}
