using LogAnalytics.Shared.Services;
using Microsoft.JSInterop;

namespace LogAnalytics.Client.Services;

public class ClientThemeService : IThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isDarkMode;

    public event Action? ThemeChanged;
    public bool IsDarkMode => _isDarkMode;

    public ClientThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var savedTheme = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "fluent-theme");
            if (savedTheme != null)
            {
                _isDarkMode = savedTheme == "dark";
            }
            else
            {
                var isDark = await _jsRuntime.InvokeAsync<bool>("window.matchMedia", "(prefers-color-scheme: dark)").AsTask();
                _isDarkMode = isDark;
            }
        }
        catch
        {
            _isDarkMode = false;
        }
    }

    public async Task ToggleThemeAsync()
    {
        _isDarkMode = !_isDarkMode;
        await SaveAndApplyThemeAsync();
    }

    public async Task SetThemeAsync(bool isDarkMode)
    {
        if (_isDarkMode != isDarkMode)
        {
            _isDarkMode = isDarkMode;
            await SaveAndApplyThemeAsync();
        }
    }

    private async Task SaveAndApplyThemeAsync()
    {
        try
        {
            var theme = _isDarkMode ? "dark" : "light";
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "fluent-theme", theme);
            ThemeChanged?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating theme: {ex.Message}");
        }
    }
}