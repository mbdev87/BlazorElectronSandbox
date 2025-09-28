using LogAnalytics.Shared;

namespace LogAnalytics.Desktop;

public class DesktopEnvironmentInfoService : IEnvironmentInfoService
{
    public string GetMode() => "Desktop";
}