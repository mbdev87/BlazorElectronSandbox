using LogAnalytics.Shared.Services;

namespace LogAnalytics.Server.Services;

public class ServerRuntimeEnvironment : IRuntimeEnvironment
{
    public RuntimeType Type => RuntimeType.Server;
    public bool IsElectron => false;
    public bool IsWebAssembly => false;
    public bool IsServer => true;

    public string GetDisplayName() => "Server (ASP.NET Core)";
}