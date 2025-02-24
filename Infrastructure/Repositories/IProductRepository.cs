using Application.Repositories;
using Domain;

namespace Infrastructure.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<List<Product?>> getAllProductAsync();
    }
}
