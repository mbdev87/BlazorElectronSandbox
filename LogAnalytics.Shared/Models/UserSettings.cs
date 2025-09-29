namespace LogAnalytics.Shared.Models;

public record UserSettings
{
    public bool IsDarkMode { get; init; } = false;
    public string Language { get; init; } = "en";
    public int RefreshInterval { get; init; } = 30;
    public bool EnableNotifications { get; init; } = true;
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;
}