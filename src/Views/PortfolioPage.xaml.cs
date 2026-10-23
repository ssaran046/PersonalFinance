using PersonalFinance.ViewModels;

namespace PersonalFinance.Views;

public partial class PortfolioPage : ContentPage
{
	public PortfolioPage(PortfolioViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}