using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinance.Models;
using PersonalFinance.Services.Interfaces;
using PersonalFinance.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PersonalFinance.ViewModels;

public partial class AssetTypesViewModel : ObservableObject
{
    private readonly IAssetTypeService _assetTypeService;

    public AssetTypesViewModel(
        IAssetTypeService assetTypeService)
    {
        _assetTypeService = assetTypeService;

        AssetTypeTappedCommand =
            new Command<AssetType>(OnAssetTypeTapped);
    }

    public ObservableCollection<AssetType> AssetTypes { get; } = [];

    public ICommand AssetTypeTappedCommand { get; }

    [RelayCommand]
    private async Task LoadAsync()
    {
        AssetTypes.Clear();

        var assetTypes =
            await _assetTypeService.GetAssetTypesAsync();

        foreach (var assetType in assetTypes)
        {
            AssetTypes.Add(assetType);
        }
    }

    [RelayCommand]
    private async Task AddAssetTypeAsync()
    {
        await Shell.Current.GoToAsync(
            nameof(AddEditAssetTypePage));
    }

    private async void OnAssetTypeTapped(
        AssetType assetType)
    {
        if (assetType == null)
            return;

        await Shell.Current.GoToAsync(
            $"{nameof(AddEditAssetTypePage)}?AssetTypeId={assetType.Id}");
    }
}