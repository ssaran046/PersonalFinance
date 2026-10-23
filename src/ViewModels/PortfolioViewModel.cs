using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinance.Models;
using PersonalFinance.Services.Interfaces;
using PersonalFinance.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PersonalFinance.ViewModels;

public partial class PortfolioViewModel : ObservableObject
{
    private readonly IAssetService _assetService;

    public PortfolioViewModel(
        IAssetService assetService)
    {
        _assetService = assetService;

        AssetTappedCommand =
            new Command<Asset>(OnAssetTapped);
    }

    public ObservableCollection<Asset> Assets { get; } = [];

    public ObservableCollection<AssetAllocationSummary>
        AssetAllocations
    { get; } = [];

    [ObservableProperty]
    private decimal totalPortfolioValue;

    public ICommand AssetTappedCommand { get; }

    [RelayCommand]
    private async Task LoadAsync()
    {
        Assets.Clear();
        AssetAllocations.Clear();

        var assets =
            await _assetService.GetAssetsAsync();

        foreach (var asset in assets)
        {
            Assets.Add(asset);
        }

        TotalPortfolioValue =
            await _assetService.GetTotalPortfolioAsync();

        var allocations =
            await _assetService.GetAssetAllocationAsync();

        foreach (var allocation in allocations)
        {
            AssetAllocations.Add(allocation);
        }
    }

    [RelayCommand]
    private async Task AddAssetAsync()
    {
        await Shell.Current.GoToAsync(
            nameof(AddEditAssetPage));
    }

    private async void OnAssetTapped(
        Asset asset)
    {
        if (asset == null)
            return;

        await Shell.Current.GoToAsync(
            $"{nameof(AddEditAssetPage)}?AssetId={asset.Id}");
    }
}