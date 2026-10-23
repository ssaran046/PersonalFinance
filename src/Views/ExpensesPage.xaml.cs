using PersonalFinance.ViewModels;

namespace PersonalFinance.Views;

public partial class ExpensesPage : ContentPage
{
	public ExpensesPage(ExpensesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}