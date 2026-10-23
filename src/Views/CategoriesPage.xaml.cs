using PersonalFinance.ViewModels;

namespace PersonalFinance.Views;

public partial class CategoriesPage : ContentPage
{
	public CategoriesPage(CategoriesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}