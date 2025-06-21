using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int id);

        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<IEnumerable<Category>> GetInactiveCategoriesAsync();
        Task<bool> DeactivateCategoryAsync(int id);
    }
} 