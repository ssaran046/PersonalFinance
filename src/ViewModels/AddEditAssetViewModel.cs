using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinance.Models;
using PersonalFinance.Services.Interfaces;
using System.Collections.ObjectModel;

namespace PersonalFinance.ViewModels;

[QueryProperty(nameof(AssetId), nameof(AssetId))]
public partial class AddEditAssetViewModel : ObservableObject
{
    private readonly IAssetService _assetService;
    private readonly IAssetTypeService _assetTypeService;

    private Asset? _asset;
    private bool _assetTypesLoaded;

    public AddEditAssetViewModel(
        IAssetService assetService,
        IAssetTypeService assetTypeService)
    {
        _assetService = assetService;
        _assetTypeService = assetTypeService;

        _ = EnsureAssetTypesLoadedAsync();
    }

    public ObservableCollection<AssetType> AssetTypes { get; } = [];

    [ObservableProperty]
    private int assetId;

    [ObservableProperty]
    private bool isEditMode;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private AssetType? selectedAssetType;

    [ObservableProperty]
    private decimal amount;

    [ObservableProperty]
    private string notes = string.Empty;

    public string PageTitle =>
        IsEditMode ? "Edit Asset" : "Add Asset";

    partial void OnIsEditModeChanged(bool value)
    {
        OnPropertyChanged(nameof(PageTitle));
    }

    partial void OnAssetIdChanged(int value)
    {
        IsEditMode = value > 0;

        if (value > 0)
        {
            _ = LoadAssetAsync();
        }
        else
        {
            _ = EnsureAssetTypesLoadedAsync();
        }
    }

    private async Task EnsureAssetTypesLoadedAsync()
    {
        if (_assetTypesLoaded)
            return;

        AssetTypes.Clear();

        var assetTypes =
            await _assetTypeService.GetAssetTypesAsync();

        foreach (var assetType in assetTypes)
        {
            AssetTypes.Add(assetType);
        }

        if (SelectedAssetType == null &&
            AssetTypes.Count > 0)
        {
            SelectedAssetType = AssetTypes[0];
        }

        _assetTypesLoaded = true;
    }

    private async Task LoadAssetAsync()
    {
        await EnsureAssetTypesLoadedAsync();

        _asset =
            await _assetService.GetAssetByIdAsync(
                AssetId);

        if (_asset == null)
            return;

        Name = _asset.Name;
        Amount = _asset.Amount;
        Notes = _asset.Notes ?? string.Empty;

        SelectedAssetType = AssetTypes
            .FirstOrDefault(
                x => x.Id == _asset.AssetTypeId);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        Name = Name.Trim();

        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert(
                "Validation",
                "Asset name is required.",
                "OK");

            return;
        }

        if (SelectedAssetType == null)
        {
            await Shell.Current.DisplayAlert(
                "Validation",
                "Please select an asset type.",
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

        if (AssetId > 0)
        {
            if (_asset == null)
                return;

            _asset.Name = Name;
            _asset.AssetTypeId = SelectedAssetType.Id;
            _asset.AssetType = null;
            _asset.Amount = Amount;
            _asset.Notes = Notes;

            await _assetService.UpdateAssetAsync(
                _asset);
        }
        else
        {
            await _assetService.AddAssetAsync(
                new Asset
                {
                    Name = Name,
                    AssetTypeId = SelectedAssetType.Id,
                    Amount = Amount,
                    Notes = Notes
                });
        }

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (AssetId <= 0)
            return;

        bool confirm =
            await Shell.Current.DisplayAlert(
                "Delete Asset",
                $"Delete '{Name}'?",
                "Delete",
                "Cancel");

        if (!confirm)
            return;

        await _assetService.DeleteAssetAsync(
            AssetId);

        await Shell.Current.GoToAsync("..");
    }
}