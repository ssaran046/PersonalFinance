using PersonalFinance.ViewModels;

namespace PersonalFinance.Views;

public partial class AddEditAssetTypePage : ContentPage
{
	public AddEditAssetTypePage(AddEditAssetTypeViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}