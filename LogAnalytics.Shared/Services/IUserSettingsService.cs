using LogAnalytics.Shared.Models;

namespace LogAnalytics.Shared.Services;

public interface IUserSettingsService
{
    event Action<UserSettings>? SettingsChanged;

    UserSettings Current { get; }

    Task<UserSettings> LoadSettingsAsync();
    Task SaveSettingsAsync(UserSettings settings);
    Task UpdateThemeAsync(bool isDarkMode);
    Task<bool> GetThemeAsync();
}