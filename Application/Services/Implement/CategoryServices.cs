using Application.Interfaces;
using Application;
using AutoMapper;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Domain.Model;
using Microsoft.IdentityModel.Tokens;
using Application.Services;

namespace WebAPI.Services
{
    public class CategoryServices : ICategoryServices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        private readonly ICropRepository _cropRepository;
        public CategoryServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper, ICropRepository cropRepository)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
            _cropRepository = cropRepository;
        }

        public async Task<Category> CreateCategoryAsync(string categoryName)
        {
            Category category = new Category
            {
                CategoryName = categoryName,
            };
            await _unitOfWork.categoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(long id)
        {
            var checkCategory = await _unitOfWork.categoryRepository.GetByIdAsync(id);
            if (checkCategory == null)
            {
                throw new Exception("Category not found.");
            }
            await _unitOfWork.categoryRepository.DeleteAsync(checkCategory);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result < 1)
                return true;
            else
                return false;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var result = await _unitOfWork.categoryRepository.GetAllAsync();
            if (result.IsNullOrEmpty())
            {
                throw new Exception("No categories found.");
            }
            else
            {
                // Map dữ liệu sang DTO
                var resultView = _mapper.Map<List<Category>>(result);
                return resultView;
            }
        }

        public Task<Category> GetCategoryByIdAsync(long id)
        {
            var result = _unitOfWork.categoryRepository.GetByIdAsync(id);
            if (result == null)
            {
                throw new Exception("Category not found.");
            }
            else
                return result;
        }

        public async Task<Category> UpdateCategoryAsync(long id, string Name)
        {
            var checkCategory = await _unitOfWork.categoryRepository.GetByIdAsync(id);
            if (checkCategory == null)
            {
                throw new Exception("Category not found.");
            }
            else
            {
                checkCategory.CategoryName = Name;
                await _unitOfWork.categoryRepository.UpdateAsync(checkCategory);
                var result = _unitOfWork.SaveChangesAsync();
                if (result == null)
                {
                    throw new Exception("Update failed.");
                }
                else
                {
                    return checkCategory;
                }
            }
        }
    }
}
