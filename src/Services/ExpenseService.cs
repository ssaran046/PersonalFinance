using Microsoft.EntityFrameworkCore;
using PersonalFinance.Data;
using PersonalFinance.Models;
using PersonalFinance.Services.Interfaces;

namespace PersonalFinance.Services;

public class ExpenseService : IExpenseService
{
    private readonly IDbContextFactory<FinanceDbContext> _dbContextFactory;

    public ExpenseService(IDbContextFactory<FinanceDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<Expense>> GetExpensesAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Expenses
            .Include(x => x.Category)
            .OrderByDescending(x => x.Date.Date)
            .ToListAsync();
    }

    public async Task<Expense?> GetExpenseByIdAsync(int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Expenses
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddExpenseAsync(Expense expense)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        context.Expenses.Add(expense);

        await context.SaveChangesAsync();
    }

    public async Task UpdateExpenseAsync(Expense expense)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var existingExpense = await context.Expenses
            .FirstOrDefaultAsync(x => x.Id == expense.Id);

        if (existingExpense == null)
            return;

        existingExpense.Name = expense.Name;
        existingExpense.CategoryId = expense.CategoryId;
        existingExpense.Amount = expense.Amount;
        existingExpense.Date = expense.Date;
        existingExpense.Notes = expense.Notes;

        await context.SaveChangesAsync();
    }

    public async Task DeleteExpenseAsync(int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var expense = await context.Expenses
            .FirstOrDefaultAsync(x => x.Id == id);

        if (expense == null)
            return;

        context.Expenses.Remove(expense);

        await context.SaveChangesAsync();
    }

    public async Task<decimal> GetTodayExpenseAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var expenses = await context.Expenses
            .Where(x => x.Date.Date == DateTime.Today)
            .ToListAsync();

        return expenses.Sum(x => x.Amount);
    }

    public async Task<decimal> GetThisMonthExpenseAsync()
    {
        var today = DateTime.Today;

        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var expenses = await context.Expenses
            .Where(x => x.Date.Month == today.Month &&
                        x.Date.Year == today.Year)
            .ToListAsync();

        return expenses.Sum(x => x.Amount);
    }

    public async Task<decimal> GetThisYearExpenseAsync()
    {
        var year = DateTime.Today.Year;

        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var expenses = await context.Expenses
            .Where(x => x.Date.Year == year)
            .ToListAsync();

        return expenses.Sum(x => x.Amount);
    }

    public async Task<decimal> GetAverageDailyExpenseAsync()
    {
        var thisYearExpense =
            await GetThisYearExpenseAsync();

        return thisYearExpense /
               DateTime.Today.DayOfYear;
    }

    public async Task<decimal> GetAverageMonthlyExpenseAsync()
    {
        var thisYearExpense =
            await GetThisYearExpenseAsync();

        return thisYearExpense /
               DateTime.Today.Month;
    }

    public async Task<List<CategorySummary>>
        GetMonthlyCategorySummaryAsync()
    {
        var today = DateTime.Today;

        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var expenses = await context.Expenses
            .Include(x => x.Category)
            .Where(x => x.Date.Month == today.Month &&
                        x.Date.Year == today.Year)
            .ToListAsync();

        return expenses
            .GroupBy(x => x.Category!.Name)
            .Select(g => new CategorySummary
            {
                Category = g.Key,
                Amount = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.Amount)
            .ToList();
    }

    public async Task<MonthlyExpenseSummary> GetMonthlySummaryAsync(
    int year,
    int month)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var expenses = await context.Expenses
            .Where(x =>
                x.Date.Year == year &&
                x.Date.Month == month)
            .ToListAsync();

        var totalAmount = expenses.Sum(x => x.Amount);

        var daysInMonth =
            DateTime.DaysInMonth(year, month);

        return new MonthlyExpenseSummary
        {
            TotalAmount = totalAmount,
            DailyAverage = daysInMonth > 0
                ? totalAmount / daysInMonth
                : 0
        };
    }
}