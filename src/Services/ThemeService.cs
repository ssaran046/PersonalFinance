using PersonalFinance.Common;
using PersonalFinance.Services.Interfaces;

namespace PersonalFinance.Services;

public class ThemeService : IThemeService
{
    public AppTheme CurrentTheme
    {
        get
        {
            var savedTheme = Preferences.Default.Get(
                PreferenceKeys.AppTheme,
                nameof(AppTheme.Unspecified));

            return Enum.TryParse<AppTheme>(savedTheme, out var appTheme)
                ? appTheme
                : AppTheme.Unspecified;
        }
    }

    public void ApplyTheme(AppTheme appTheme)
    {
        var currentApplication = Application.Current;

        if (currentApplication == null)
            return;

        currentApplication.UserAppTheme = appTheme;

        Preferences.Default.Set(
            PreferenceKeys.AppTheme,
            appTheme.ToString());
    }
}
