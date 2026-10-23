using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinance.Services.Interfaces;
using PersonalFinance.Views;

namespace PersonalFinance.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IBackupRestoreService _backupRestoreService;
    private readonly IThemeService _themeService;

    public SettingsViewModel(
        IBackupRestoreService backupRestoreService,
        IThemeService themeService)
    {
        _backupRestoreService = backupRestoreService;
        _themeService = themeService;
    }

    [RelayCommand]
    private async Task OpenThemeOptionsAsync()
    {
        string action = await Shell.Current.DisplayActionSheet(
            "Themes",
            "Cancel",
            null,
            "Light",
            "Dark",
            "Use Device Settings");

        if (action == "Light")
        {
            _themeService.ApplyTheme(AppTheme.Light);
            return;
        }

        if (action == "Dark")
        {
            _themeService.ApplyTheme(AppTheme.Dark);
            return;
        }

        if (action == "Use Device Settings")
        {
            _themeService.ApplyTheme(AppTheme.Unspecified);
        }
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