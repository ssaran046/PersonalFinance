using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinance.Models;
using PersonalFinance.Services.Interfaces;
using PersonalFinance.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PersonalFinance.ViewModels;

public partial class CategoriesViewModel : ObservableObject
{
    private readonly ICategoryService _categoryService;

    public CategoriesViewModel(ICategoryService categoryService)
    {
        _categoryService = categoryService;

        CategoryTappedCommand =
            new Command<Category>(OnCategoryTapped);
    }

    public ObservableCollection<Category> Categories { get; } = [];

    public ICommand CategoryTappedCommand { get; }

    [RelayCommand]
    private async Task LoadAsync()
    {
        Categories.Clear();

        var categories =
            await _categoryService.GetCategoriesAsync();

        foreach (var category in categories)
        {
            Categories.Add(category);
        }
    }

    [RelayCommand]
    private async Task AddCategoryAsync()
    {
        await Shell.Current.GoToAsync(
            nameof(AddEditCategoryPage));
    }

    private async void OnCategoryTapped(Category category)
    {
        if (category == null)
            return;

        await Shell.Current.GoToAsync(
            $"{nameof(AddEditCategoryPage)}?CategoryId={category.Id}");
    }
}