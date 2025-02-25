using Application.Repositories;
using Domain;

namespace Infrastructure.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<int> CountAsync(); // Đếm tổng số sản phẩm
        Task<IEnumerable<Product>> GetPagedAsync(int pageIndex, int pageSize);
        Task<Product?> GetProductByCurrentId(int productId);
        Task<List<Product?>> GetProductByNameAsync(string productName);
    }
}
