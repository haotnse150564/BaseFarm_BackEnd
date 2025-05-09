﻿using Application.Repositories;
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
    }
}
