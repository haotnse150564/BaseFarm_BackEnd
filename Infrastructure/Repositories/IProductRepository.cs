using Application.Commons;
using Application.Repositories;
using Domain.Enum;
using Domain.Model;

namespace Infrastructure.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<int> CountAsync(); // Đếm tổng số sản phẩm
        Task<IEnumerable<Product>> GetPagedAsync(int pageIndex, int pageSize);
        Task<int> CountByNameAsync(string productName);
        Task<List<Product>> GetPagedByNameAsync(string productName, int pageIndex, int pageSize);
        Task<Product?> GetProductById(long productId);
        Task<List<Product?>> GetProductByNameAsync(string productName);
        Task<bool> ExistsByNameAsync(string name);
        Task<Pagination<Product>> GetFilteredProductsAsync(
            int pageIndex,
            int pageSize,
            Status? status = null,
            long? categoryId = null,
            bool sortByStockAsc = true);
        Task UpdateStockByOrderAsync(long productId, int quantityToReduce);
        Task <List<Product>> GetProductTotals();
    }
}
