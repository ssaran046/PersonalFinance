using PersonalFinance.Models;

namespace PersonalFinance.Services.Interfaces;

public interface ICategoryService
{
    Task<List<Category>> GetCategoriesAsync();

    Task<Category?> GetCategoryByIdAsync(int id);

    Task<Category?> GetDefaultCategoryAsync();

    Task<int> GetExpenseCountAsync(int categoryId);

    Task<bool> CategoryExistsAsync(
        string name,
        int excludeId = 0);

    Task AddCategoryAsync(Category category);

    Task UpdateCategoryAsync(Category category);

    Task DeleteEmptyCategoryAsync(int categoryId);

    Task DeleteCategoryAsync(
        int categoryId,
        int destinationCategoryId);

    Task DeleteCategoryWithExpensesAsync(
        int categoryId);
}