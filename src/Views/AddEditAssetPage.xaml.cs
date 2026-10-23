using PersonalFinance.ViewModels;

namespace PersonalFinance.Views;

public partial class AddEditAssetPage : ContentPage
{
	public AddEditAssetPage(AddEditAssetViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}