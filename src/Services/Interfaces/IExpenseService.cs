using PersonalFinance.Models;

namespace PersonalFinance.Services.Interfaces;

public interface IExpenseService
{
    Task<List<Expense>> GetExpensesAsync();

    Task AddExpenseAsync(Expense expense);

    Task<decimal> GetTodayExpenseAsync();

    Task<decimal> GetThisMonthExpenseAsync();

    Task<decimal> GetThisYearExpenseAsync();

    Task<decimal> GetAverageDailyExpenseAsync();

    Task<decimal> GetAverageMonthlyExpenseAsync();

    Task<Expense?> GetExpenseByIdAsync(int id);

    Task UpdateExpenseAsync(Expense expense);

    Task DeleteExpenseAsync(int id);

    Task<List<CategorySummary>> GetMonthlyCategorySummaryAsync();

    Task<MonthlyExpenseSummary> GetMonthlySummaryAsync(int year, int month);
}