using System.Text.Json;
using LogAnalytics.Shared.Models;
using LogAnalytics.Shared.Services;

namespace LogAnalytics.Server.Services;

public class FileUserSettingsService : IUserSettingsService
{
    private readonly string _settingsPath;
    private UserSettings _currentSettings;
    private readonly JsonSerializerOptions _jsonOptions;

    public event Action<UserSettings>? SettingsChanged;
    public UserSettings Current => _currentSettings;

    public FileUserSettingsService()
    {
        // Store settings in user's local app data folder
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "LogAnalytics");
        Directory.CreateDirectory(appFolder);
        _settingsPath = Path.Combine(appFolder, "user-settings.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        _currentSettings = new UserSettings();
    }

    public async Task<UserSettings> LoadSettingsAsync()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = await File.ReadAllTextAsync(_settingsPath);
                var settings = JsonSerializer.Deserialize<UserSettings>(json, _jsonOptions);
                if (settings != null)
                {
                    _currentSettings = settings;
                    SettingsChanged?.Invoke(_currentSettings);
                    return _currentSettings;
                }
            }
        }
        catch (Exception ex)
        {
            // Log error but don't crash - use defaults
            Console.WriteLine($"Error loading user settings: {ex.Message}");
        }

        // Return defaults if loading failed
        _currentSettings = new UserSettings();
        return _currentSettings;
    }

    public async Task SaveSettingsAsync(UserSettings settings)
    {
        try
        {
            var updatedSettings = settings with { LastUpdated = DateTime.UtcNow };
            var json = JsonSerializer.Serialize(updatedSettings, _jsonOptions);
            await File.WriteAllTextAsync(_settingsPath, json);

            _currentSettings = updatedSettings;
            SettingsChanged?.Invoke(_currentSettings);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving user settings: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateThemeAsync(bool isDarkMode)
    {
        var settings = _currentSettings with { IsDarkMode = isDarkMode };
        await SaveSettingsAsync(settings);
    }

    public Task<bool> GetThemeAsync()
    {
        return Task.FromResult(_currentSettings.IsDarkMode);
    }
}