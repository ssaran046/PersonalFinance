using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonalFinance.Data;
using PersonalFinance.Models;
using PersonalFinance.Services;
using PersonalFinance.Services.Interfaces;
using PersonalFinance.ViewModels;
using PersonalFinance.Views;

namespace PersonalFinance;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        string dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "finance.db");

        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddDbContextFactory<FinanceDbContext>(
            options => options.UseSqlite($"Data Source={dbPath}"));

        // Services
        builder.Services.AddScoped<IExpenseService, ExpenseService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IAssetService, AssetService>();
        builder.Services.AddScoped<IAssetTypeService, AssetTypeService>();
        builder.Services.AddScoped<IBackupRestoreService, BackupRestoreService>();

        // ViewModels
        builder.Services.AddTransient<ExpensesViewModel>();
        builder.Services.AddTransient<AddExpenseViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<CategoriesViewModel>();
        builder.Services.AddTransient<AddEditCategoryViewModel>();
        builder.Services.AddTransient<AssetTypesViewModel>();
        builder.Services.AddTransient<AddEditAssetTypeViewModel>();
        builder.Services.AddTransient<AddEditAssetViewModel>();
        builder.Services.AddTransient<PortfolioViewModel>();

        // Views
        builder.Services.AddTransient<ExpensesPage>();
        builder.Services.AddTransient<AddExpensePage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<CategoriesPage>();
        builder.Services.AddTransient<AddEditCategoryPage>();
        builder.Services.AddTransient<AssetTypesPage>();
        builder.Services.AddTransient<AddEditAssetTypePage>();
        builder.Services.AddTransient<AddEditAssetPage>();
        builder.Services.AddTransient<PortfolioPage>();

        var app = builder.Build();

        var dbContextFactory =
            app.Services.GetRequiredService<IDbContextFactory<FinanceDbContext>>();

        using var context =
            dbContextFactory.CreateDbContext();

        context.Database.EnsureCreated();

        DatabaseSeeder.Seed(context);

        return app;
    }
}
