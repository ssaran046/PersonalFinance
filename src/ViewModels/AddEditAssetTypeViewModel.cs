using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinance.Models;
using PersonalFinance.Services.Interfaces;

namespace PersonalFinance.ViewModels;

[QueryProperty(nameof(AssetTypeId), nameof(AssetTypeId))]
public partial class AddEditAssetTypeViewModel : ObservableObject
{
    private readonly IAssetTypeService _assetTypeService;

    public AddEditAssetTypeViewModel(
        IAssetTypeService assetTypeService)
    {
        _assetTypeService = assetTypeService;
    }

    [ObservableProperty]
    private int assetTypeId;

    [ObservableProperty]
    private bool isEditMode;

    [ObservableProperty]
    private string name = string.Empty;

    public string PageTitle =>
        IsEditMode
            ? "Edit Asset Type"
            : "Add Asset Type";

    partial void OnAssetTypeIdChanged(int value)
    {
        IsEditMode = value > 0;

        if (value > 0)
        {
            _ = LoadAssetTypeAsync();
        }
    }

    partial void OnIsEditModeChanged(bool value)
    {
        OnPropertyChanged(nameof(PageTitle));
    }

    private async Task LoadAssetTypeAsync()
    {
        var assetType =
            await _assetTypeService.GetAssetTypeByIdAsync(
                AssetTypeId);

        if (assetType == null)
            return;

        Name = assetType.Name;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        Name = Name.Trim();

        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert(
                "Validation",
                "Asset type name is required.",
                "OK");

            return;
        }

        if (await _assetTypeService.AssetTypeExistsAsync(
                Name,
                AssetTypeId))
        {
            await Shell.Current.DisplayAlert(
                "Validation",
                "Asset type already exists.",
                "OK");

            return;
        }

        if (AssetTypeId > 0)
        {
            var assetType = await _assetTypeService.GetAssetTypeByIdAsync(AssetTypeId);

            if (assetType == null)
                return;

            assetType.Name = Name;

            await _assetTypeService.UpdateAssetTypeAsync(assetType);
        }
        else
        {
            await _assetTypeService.AddAssetTypeAsync(
                new AssetType
                {
                    Name = Name
                });
        }

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (AssetTypeId <= 0)
            return;

        bool confirm =
            await Shell.Current.DisplayAlert(
                "Delete Asset Type",
                $"Delete '{Name}'?",
                "Delete",
                "Cancel");

        if (!confirm)
            return;

        await _assetTypeService.DeleteAssetTypeAsync(
            AssetTypeId);

        await Shell.Current.GoToAsync("..");
    }
}