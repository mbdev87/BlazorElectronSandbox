namespace LogAnalytics.Shared.Services;

public interface IThemeService
{
    event Action? ThemeChanged;
    bool IsDarkMode { get; }

    Task InitializeAsync();
    Task ToggleThemeAsync();
    Task SetThemeAsync(bool isDarkMode);
}