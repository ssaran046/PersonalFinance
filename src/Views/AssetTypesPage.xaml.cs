using PersonalFinance.ViewModels;

namespace PersonalFinance.Views;

public partial class AssetTypesPage : ContentPage
{
	public AssetTypesPage(AssetTypesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}