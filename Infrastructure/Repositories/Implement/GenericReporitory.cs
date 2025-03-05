using Application.Commons;
using Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Implement
{

    public class GenericRepository<TModel> : IGenericRepository<TModel> where TModel : class
    {
        protected DbSet<TModel> _dbSet;
        protected AppDbContext _context;
        public GenericRepository()
        {
            _context = new AppDbContext();
            _dbSet = _context.Set<TModel>();
        }

        public virtual async Task AddAsync(TModel model)
        {
            await _dbSet.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task<TModel?> FirstOrDefaultAsync(Expression<Func<TModel, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        public virtual void AddAttach(TModel model)
        {
            _dbSet.Attach(model).State = EntityState.Added;
        }

        public virtual void AddEntry(TModel model)
        {
            _dbSet.Entry(model).State = EntityState.Added;
        }

        public virtual async Task AddRangeAsync(List<TModel> models)
        {
            await _dbSet.AddRangeAsync(models);
        }

        public virtual async Task<List<TModel>> GetAllAsync() => await _dbSet.ToListAsync();

        /// <summary>
        /// The function return list of Tmodel with an include method.
        /// Example for user we want to include the relation Role: 
        /// + GetAllAsync(user => user.Include(x => x.Role));
        /// </summary>
        /// <param name="include"> The linq expression for include relations we want. </param>
        /// <returns> Return the list of TModel include relations. </returns>
        public virtual async Task<List<TModel>> GetAllAsync(Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>? include = null)
        {
            IQueryable<TModel> query = _dbSet;
            if (include != null)
            {
                query = include(query);
            }
            return await query.ToListAsync();
        }

        public virtual async Task<TModel?> GetByIdAsync(long id) => await _dbSet.FindAsync(id);

        public void Update(TModel? model)
        {
            _dbSet.Update(model);
        }

        public async Task<int> UpdateAsync(TModel model)
        {
            var tracker = _context.Attach(model);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public void UpdateRange(List<TModel> models)
        {
            _dbSet.UpdateRange(models);
        }

        // Implement to pagination method
        public async Task<Pagination<TModel>> ToPaginationAsync(int pageIndex = 1, int pageSize = 10)
        {
            // Lấy tổng số lượng phần tử trong bảng
            var itemCount = await _dbSet.CountAsync();

            // Nếu không có dữ liệu, trả về danh sách rỗng
            if (itemCount == 0)
            {
                return new Pagination<TModel>()
                {
                    PageSize = pageSize,
                    TotalItemCount = 0,
                    PageIndex = 1, 
                    Items = new List<TModel>()
                };
            }

            // Đảm bảo pageSize > 0
            pageSize = Math.Max(1, pageSize);

            // Tính số trang tối đa
            var totalPagesCount = (int)Math.Ceiling((double)itemCount / pageSize);

            // Đảm bảo pageIndex nằm trong khoảng hợp lệ (tối thiểu 1, tối đa totalPagesCount)
            pageIndex = Math.Max(1, Math.Min(pageIndex, totalPagesCount));

            // Tạo đối tượng Pagination
            var result = new Pagination<TModel>()
            {
                PageSize = pageSize,
                TotalItemCount = itemCount,
                PageIndex = pageIndex,
            };

            // Lấy dữ liệu từ database theo trang (chỉnh pageIndex - 1 để bắt đầu từ 1)
            var items = await _dbSet
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            // Gán danh sách vào kết quả
            result.Items = items;

            return result;
        }


        public async Task<TModel> CloneAsync(TModel model)
        {
            _dbSet.Entry(model).State = EntityState.Detached;
            var values = _dbSet.Entry(model).CurrentValues.Clone().ToObject() as TModel;
            return values;
        }

        //Delete
        public async Task<bool> DeleteAsync(TModel model)
        {
           _dbSet.Entry(model).State = EntityState.Deleted;
           return true;
        }

    }
}
