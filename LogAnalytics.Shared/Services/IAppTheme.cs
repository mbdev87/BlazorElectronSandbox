using Microsoft.FluentUI.AspNetCore.Components;

namespace LogAnalytics.Shared.Services;

public interface IAppTheme
{
    DesignThemeModes Mode { get; set; }
    Task SetModeAsync(DesignThemeModes newMode);
}