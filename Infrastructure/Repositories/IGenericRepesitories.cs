using Application.Commons;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;


namespace Application.Repositories
{
    public interface IGenericRepository<TModel> where TModel : class
    {
        Task<TModel> CloneAsync(TModel model);
        Task<List<TModel>> GetAllAsync();
        Task<List<TModel>> GetAllAsync(Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>? include = null);

        Task<TModel?> GetByIdAsync(long id);

        Task AddAsync(TModel model);

        void AddAttach(TModel model);
        void AddEntry(TModel model);
        void Update(TModel model);
        Task<int> UpdateAsync(TModel model);
        void UpdateRange(List<TModel> models);

        Task AddRangeAsync(List<TModel> models);

        // Add paging method to generic interface 
        Task<Pagination<TModel>> ToPaginationAsync(int pageIndex = 1, int pageSize = 10);
        Task<bool> DeleteAsync(TModel model);
        Task<TModel?> FirstOrDefaultAsync(Expression<Func<TModel, bool>> predicate);
    }
}
