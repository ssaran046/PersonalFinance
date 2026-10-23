using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinance.Models;
using PersonalFinance.Services.Interfaces;

namespace PersonalFinance.ViewModels;

[QueryProperty(nameof(CategoryId), nameof(CategoryId))]
public partial class AddEditCategoryViewModel : ObservableObject
{
    private readonly ICategoryService _categoryService;

    public AddEditCategoryViewModel(
        ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [ObservableProperty]
    private int categoryId;

    [ObservableProperty]
    private bool isEditMode;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private bool canDelete;

    public string PageTitle =>
        IsEditMode ? "Edit Category" : "Add Category";

    partial void OnCategoryIdChanged(int value)
    {
        IsEditMode = value > 0;

        if (value > 0)
        {
            _ = LoadCategoryAsync();
        }
    }

    partial void OnIsEditModeChanged(bool value)
    {
        OnPropertyChanged(nameof(PageTitle));
    }

    private async Task LoadCategoryAsync()
    {
        var category =
            await _categoryService.GetCategoryByIdAsync(
                CategoryId);

        if (category == null)
            return;

        Name = category.Name;

        CanDelete = !category.IsDefault;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        Name = Name.Trim();

        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert(
                "Validation",
                "Category name is required.",
                "OK");

            return;
        }

        if (await _categoryService.CategoryExistsAsync(
                Name,
                CategoryId))
        {
            await Shell.Current.DisplayAlert(
                "Validation",
                "Category already exists.",
                "OK");

            return;
        }

        if (CategoryId > 0)
        {
            var category =
                await _categoryService.GetCategoryByIdAsync(
                    CategoryId);

            if (category == null)
                return;

            if (category.IsDefault &&
                !string.Equals(
                    category.Name,
                    Name,
                    StringComparison.OrdinalIgnoreCase))
            {
                await Shell.Current.DisplayAlert(
                    "Not Allowed",
                    "Default category cannot be renamed.",
                    "OK");

                return;
            }

            category.Name = Name;

            await _categoryService.UpdateCategoryAsync(
                category);
        }
        else
        {
            await _categoryService.AddCategoryAsync(
                new Category
                {
                    Name = Name
                });
        }

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (CategoryId <= 0)
            return;

        var category =
            await _categoryService.GetCategoryByIdAsync(CategoryId);

        if (category == null)
            return;

        if (category.IsDefault)
        {
            await Shell.Current.DisplayAlert(
                "Cannot Delete",
                "Default category cannot be deleted.",
                "OK");

            return;
        }

        var expenseCount =
            await _categoryService.GetExpenseCountAsync(CategoryId);

        if (expenseCount == 0)
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Category",
                $"Delete '{category.Name}'?",
                "Delete",
                "Cancel");

            if (!confirm)
                return;

            await _categoryService.DeleteEmptyCategoryAsync(
                category.Id);

            await Shell.Current.GoToAsync("..");
            return;
        }

        string action = await Shell.Current.DisplayActionSheet(
            $"'{category.Name}' contains {expenseCount} expense(s).",
            "Cancel",
            null,
            "Move expenses to another category",
            "Delete category and expenses");

        switch (action)
        {
            case "Move expenses to another category":

                var categories =
                    (await _categoryService.GetCategoriesAsync())
                    .Where(x => x.Id != category.Id)
                    .OrderBy(x => x.Name)
                    .ToList();

                string selectedCategoryName =
                    await Shell.Current.DisplayActionSheet(
                        "Select destination category",
                        "Cancel",
                        null,
                        categories.Select(x => x.Name).ToArray());

                if (selectedCategoryName == "Cancel")
                    return;

                var destinationCategory =
                    categories.FirstOrDefault(
                        x => x.Name == selectedCategoryName);

                if (destinationCategory == null)
                    return;

                await _categoryService.DeleteCategoryAsync(
                    category.Id,
                    destinationCategory.Id);

                break;

            case "Delete category and expenses":

                bool confirmDeleteExpenses =
                    await Shell.Current.DisplayAlert(
                        "Warning",
                        $"Delete '{category.Name}' and {expenseCount} expense(s)?",
                        "Delete",
                        "Cancel");

                if (!confirmDeleteExpenses)
                    return;

                await _categoryService.DeleteCategoryWithExpensesAsync(
                    category.Id);

                break;

            default:
                return;
        }

        await Shell.Current.GoToAsync("..");
    }
}