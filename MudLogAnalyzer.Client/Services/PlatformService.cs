namespace MudLogAnalyzer.Client.Services;

public interface IPlatformService
{
    bool IsElectron { get; }
}

public class PlatformService : IPlatformService
{
    public bool IsElectron { get; set; }
}
