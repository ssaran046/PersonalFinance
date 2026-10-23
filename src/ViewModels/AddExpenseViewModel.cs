using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinance.Models;
using PersonalFinance.Services.Interfaces;
using System.Collections.ObjectModel;

namespace PersonalFinance.ViewModels;

[QueryProperty(nameof(ExpenseId), nameof(ExpenseId))]
public partial class AddExpenseViewModel : ObservableObject
{
    private readonly IExpenseService _expenseService;
    private readonly ICategoryService _categoryService;
    private bool _categoriesLoaded;

    public AddExpenseViewModel(
        IExpenseService expenseService,
        ICategoryService categoryService)
    {
        _expenseService = expenseService;
        _categoryService = categoryService;

        _ = LoadCategoriesAsync();
    }

    public ObservableCollection<Category> Categories { get; } = [];

    [ObservableProperty]
    private int expenseId;

    [ObservableProperty]
    private bool isEditMode;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private Category? selectedCategory;

    [ObservableProperty]
    private decimal amount;

    [ObservableProperty]
    private DateTime date = DateTime.Today;

    [ObservableProperty]
    private string notes = string.Empty;

    public string PageTitle =>
        IsEditMode ? "Edit Expense" : "Add Expense";

    partial void OnIsEditModeChanged(bool value)
    {
        OnPropertyChanged(nameof(PageTitle));
    }

    partial void OnExpenseIdChanged(int value)
    {
        IsEditMode = value > 0;

        if (value > 0)
        {
            _ = LoadExpenseAsync();
        }
        else
        {
            _ = LoadCategoriesAsync();
        }
    }

    private async Task LoadCategoriesAsync()
    {
        if (_categoriesLoaded)
            return;

        Categories.Clear();

        var categories = await _categoryService.GetCategoriesAsync();

        foreach (var category in categories)
        {
            Categories.Add(category);
        }

        if (SelectedCategory == null && Categories.Count > 0)
        {
            SelectedCategory = Categories[0];
        }

        _categoriesLoaded = true;
    }

    private async Task LoadExpenseAsync()
    {
        await LoadCategoriesAsync();

        var expense = await _expenseService.GetExpenseByIdAsync(ExpenseId);

        if (expense == null)
            return;

        Name = expense.Name;
        Amount = expense.Amount;
        Date = expense.Date;
        Notes = expense.Notes ?? string.Empty;

        SelectedCategory = Categories
            .FirstOrDefault(x => x.Id == expense.CategoryId);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert(
                "Validation",
                "Expense name is required.",
                "OK");

            return;
        }

        if (SelectedCategory == null)
        {
            await Shell.Current.DisplayAlert(
                "Validation",
                "Please select a category.",
                "OK");

            return;
        }

        if (Amount <= 0)
        {
            await Shell.Current.DisplayAlert(
                "Validation",
                "Amount must be greater than 0.",
                "OK");

            return;
        }

        if (ExpenseId > 0)
        {
            var expense =
                await _expenseService.GetExpenseByIdAsync(ExpenseId);

            if (expense == null)
                return;

            expense.Name = Name;
            expense.CategoryId = SelectedCategory.Id;
            expense.Category = null;
            expense.Amount = Amount;
            expense.Date = Date;
            expense.Notes = Notes;

            await _expenseService.UpdateExpenseAsync(expense);
        }
        else
        {
            var expense = new Expense
            {
                Name = Name,
                CategoryId = SelectedCategory.Id,
                Amount = Amount,
                Date = Date,
                Notes = Notes
            };

            await _expenseService.AddExpenseAsync(expense);
        }

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (ExpenseId <= 0)
            return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Delete Expense",
            $"Delete '{Name}'?",
            "Delete",
            "Cancel");

        if (!confirm)
            return;

        await _expenseService.DeleteExpenseAsync(ExpenseId);

        await Shell.Current.GoToAsync("..");
    }
}