using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ICategoryServices
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(long id);
        Task<Category> CreateCategoryAsync(string categoryName);
        Task<Category> UpdateCategoryAsync(long id, string Name);
        Task<bool> DeleteCategoryAsync(long id);
    }
}
