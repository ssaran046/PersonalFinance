using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinance.Models;
using PersonalFinance.Services.Interfaces;
using System.Collections.ObjectModel;

namespace PersonalFinance.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IExpenseService _expenseService;

    public DashboardViewModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [ObservableProperty]
    private decimal todayExpense;

    [ObservableProperty]
    private decimal thisMonthExpense;

    [ObservableProperty]
    private decimal thisYearExpense;

    [ObservableProperty]
    private decimal averageDailyExpense;

    [ObservableProperty]
    private decimal averageMonthlyExpense;

    public ObservableCollection<CategorySummary> CategorySummaries { get; } = [];

    [RelayCommand]
    private async Task LoadAsync()
    {
        TodayExpense =
            await _expenseService.GetTodayExpenseAsync();

        ThisMonthExpense =
            await _expenseService.GetThisMonthExpenseAsync();

        ThisYearExpense =
            await _expenseService.GetThisYearExpenseAsync();

        AverageDailyExpense =
            await _expenseService.GetAverageDailyExpenseAsync();

        AverageMonthlyExpense =
            await _expenseService.GetAverageMonthlyExpenseAsync();

        var summaries =
            await _expenseService.GetMonthlyCategorySummaryAsync();

        CategorySummaries.Clear();

        foreach (var item in summaries)
        {
            CategorySummaries.Add(item);
        }
    }
}