using LogAnalytics.Shared.Services;

namespace LogAnalytics.Client.Services;

public class ClientRuntimeEnvironment : IRuntimeEnvironment
{
    public RuntimeType Type => RuntimeType.WebAssembly;
    public bool IsElectron => false;
    public bool IsWebAssembly => true;
    public bool IsServer => false;

    public string GetDisplayName() => "WebAssembly Client";
}