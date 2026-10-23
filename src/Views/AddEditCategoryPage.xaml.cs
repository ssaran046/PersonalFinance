using PersonalFinance.ViewModels;

namespace PersonalFinance.Views;

public partial class AddEditCategoryPage : ContentPage
{
	public AddEditCategoryPage(AddEditCategoryViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}