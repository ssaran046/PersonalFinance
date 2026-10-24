namespace PersonalFinance.Services.Interfaces;

public interface IThemeService
{
    AppTheme CurrentTheme { get; }

    void ApplyTheme(AppTheme appTheme);
}
