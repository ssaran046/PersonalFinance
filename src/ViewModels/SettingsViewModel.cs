using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinance.Services.Interfaces;
using PersonalFinance.Views;

namespace PersonalFinance.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IBackupRestoreService _backupRestoreService;

    public SettingsViewModel(
        IBackupRestoreService backupRestoreService)
    {
        _backupRestoreService = backupRestoreService;
    }

    [RelayCommand]
    private async Task OpenCategoriesAsync()
    {
        await Shell.Current.GoToAsync(nameof(CategoriesPage));
    }

    [RelayCommand]
    private async Task OpenAssetTypesAsync()
    {
        await Shell.Current.GoToAsync(
            nameof(AssetTypesPage));
    }

    [RelayCommand]
    private async Task OpenBackupRestoreAsync()
    {
        string action = await Shell.Current.DisplayActionSheet(
            "Backup/Restore",
            "Cancel",
            null,
            "Backup",
            "Restore");

        if (action == "Backup")
        {
            await _backupRestoreService.BackupAsync();
            return;
        }

        if (action == "Restore")
        {
            await _backupRestoreService.RestoreAsync();
        }
    }

    [RelayCommand]
    private async Task OpenAboutAsync()
    {
        await Shell.Current.DisplayAlert(
            "About",
            "Personal Finance v1.0",
            "OK");
    }
}