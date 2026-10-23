using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinance.Models;
using PersonalFinance.Services.Interfaces;
using PersonalFinance.Views;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace PersonalFinance.ViewModels;

public partial class ExpensesViewModel : ObservableObject
{
    private readonly IExpenseService _expenseService;

    public ExpensesViewModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;

        ExpenseTappedCommand =
            new Command<Expense>(OnExpenseTapped);

        LoadMonths();
    }

    [ObservableProperty]
    private string selectedMonth = string.Empty;

    [ObservableProperty]
    private decimal monthlyTotal;

    [ObservableProperty]
    private decimal dailyAverage;

    public ObservableCollection<string> Months { get; } = [];

    public ObservableCollection<ExpenseGroup> ExpenseGroups { get; } = [];

    public ICommand ExpenseTappedCommand { get; }

    partial void OnSelectedMonthChanged(string value)
    {
        _ = LoadMonthlySummaryAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        await LoadMonthlySummaryAsync();

        var expenses = await _expenseService.GetExpensesAsync();

        ExpenseGroups.Clear();

        var groups = expenses
            .GroupBy(x => x.Date.Date)
            .OrderByDescending(x => x.Key);

        foreach (var group in groups)
        {
            ExpenseGroups.Add(
                new ExpenseGroup(group.Key, group));
        }
    }

    [RelayCommand]
    private async Task AddExpenseAsync()
    {
        await Shell.Current.GoToAsync(
            nameof(AddExpensePage));
    }

    private async void OnExpenseTapped(Expense expense)
    {
        if (expense == null)
            return;

        await Shell.Current.GoToAsync(
            $"{nameof(AddExpensePage)}?ExpenseId={expense.Id}");
    }

    private void LoadMonths()
    {
        Months.Clear();

        int year = DateTime.Today.Year;

        for (int month = 1; month <= 12; month++)
        {
            Months.Add(
                new DateTime(year, month, 1)
                    .ToString("MMM yyyy"));
        }

        SelectedMonth =
            DateTime.Today.ToString("MMM yyyy");
    }

    private async Task LoadMonthlySummaryAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedMonth))
            return;

        var date = DateTime.ParseExact(
            SelectedMonth,
            "MMM yyyy",
            CultureInfo.InvariantCulture);

        var summary =
            await _expenseService.GetMonthlySummaryAsync(
                date.Year,
                date.Month);

        MonthlyTotal = summary.TotalAmount;
        DailyAverage = summary.DailyAverage;
    }
}