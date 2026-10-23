using PersonalFinance.Views;

namespace PersonalFinance
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AddExpensePage), typeof(AddExpensePage));
            Routing.RegisterRoute(nameof(CategoriesPage), typeof(CategoriesPage));
            Routing.RegisterRoute(nameof(AddEditCategoryPage), typeof(AddEditCategoryPage));
            Routing.RegisterRoute(nameof(AssetTypesPage), typeof(AssetTypesPage));
            Routing.RegisterRoute(nameof(AddEditAssetTypePage), typeof(AddEditAssetTypePage));
            Routing.RegisterRoute(nameof(AddEditAssetPage), typeof(AddEditAssetPage));
        }
    }
}
