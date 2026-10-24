using PersonalFinance.Services.Interfaces;

namespace PersonalFinance
{
    public partial class App : Application
    {
        public App(
            IThemeService themeService)
        {
            InitializeComponent();

            themeService.ApplyTheme(
                themeService.CurrentTheme);

            MainPage = new AppShell();
        }
    }
}
