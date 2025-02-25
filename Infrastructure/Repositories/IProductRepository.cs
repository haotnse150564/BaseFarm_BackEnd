using Application.Repositories;
using Domain;

namespace Infrastructure.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<List<Product?>> getAllProductAsync();
        Task<Product?> GetProductByCurrentId(int productId);
        Task<List<Product?>> GetProductByNameAsync(string productName);
    }
}
