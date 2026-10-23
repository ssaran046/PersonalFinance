using Microsoft.EntityFrameworkCore;
using PersonalFinance.Data;
using PersonalFinance.Models;
using PersonalFinance.Services.Interfaces;

namespace PersonalFinance.Services;

public class CategoryService : ICategoryService
{
    private readonly IDbContextFactory<FinanceDbContext> _dbContextFactory;

    public CategoryService(IDbContextFactory<FinanceDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Categories
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Categories
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Category?> GetDefaultCategoryAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Categories
            .FirstOrDefaultAsync(x => x.IsDefault);
    }

    public async Task<int> GetExpenseCountAsync(int categoryId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Expenses
            .CountAsync(x => x.CategoryId == categoryId);
    }

    public async Task<bool> CategoryExistsAsync(
        string name,
        int excludeId = 0)
    {
        name = name.Trim();

        await using var context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Categories.AnyAsync(
            x => x.Id != excludeId &&
                 x.Name.Trim().ToLower() ==
                 name.ToLower());
    }

    public async Task AddCategoryAsync(Category category)
    {
        category.Name = category.Name.Trim();

        await using var context = await _dbContextFactory.CreateDbContextAsync();

        context.Categories.Add(category);

        await context.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var existingCategory =
            await context.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);

        if (existingCategory == null)
            return;

        if (existingCategory.IsDefault &&
            !string.Equals(
                existingCategory.Name,
                category.Name,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Default category cannot be renamed.");
        }

        existingCategory.Name =
            category.Name.Trim();

        await context.SaveChangesAsync();
    }

    public async Task DeleteEmptyCategoryAsync(
        int categoryId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var category =
            await context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);

        if (category == null)
            return;

        if (category.IsDefault)
        {
            throw new InvalidOperationException(
                "Default category cannot be deleted.");
        }

        var expenseCount =
            await context.Expenses.CountAsync(x => x.CategoryId == categoryId);

        if (expenseCount > 0)
        {
            throw new InvalidOperationException(
                "Category contains expenses.");
        }

        context.Categories.Remove(category);

        await context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(
        int categoryId,
        int destinationCategoryId)
    {
        if (categoryId == destinationCategoryId)
        {
            throw new InvalidOperationException(
                "Source and destination categories cannot be the same.");
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var category =
            await context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);

        if (category == null)
            return;

        if (category.IsDefault)
        {
            throw new InvalidOperationException(
                "Default category cannot be deleted.");
        }

        var destinationCategory =
            await context.Categories.FirstOrDefaultAsync(x => x.Id == destinationCategoryId);

        if (destinationCategory == null)
        {
            throw new InvalidOperationException(
                "Destination category not found.");
        }

        var expenses = await context.Expenses
            .Where(x => x.CategoryId == categoryId)
            .ToListAsync();

        foreach (var expense in expenses)
        {
            expense.CategoryId = destinationCategoryId;
        }

        context.Categories.Remove(category);

        await context.SaveChangesAsync();
    }

    public async Task DeleteCategoryWithExpensesAsync(
        int categoryId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var category =
            await context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);

        if (category == null)
            return;

        if (category.IsDefault)
        {
            throw new InvalidOperationException(
                "Default category cannot be deleted.");
        }

        var expenses = await context.Expenses
            .Where(x => x.CategoryId == categoryId)
            .ToListAsync();

        context.Expenses.RemoveRange(expenses);

        context.Categories.Remove(category);

        await context.SaveChangesAsync();
    }
}