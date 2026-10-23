using PersonalFinance.ViewModels;

namespace PersonalFinance.Views;

public partial class AddExpensePage : ContentPage
{
	public AddExpensePage(AddExpenseViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}