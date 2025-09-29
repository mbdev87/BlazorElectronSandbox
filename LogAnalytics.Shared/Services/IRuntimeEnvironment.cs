namespace LogAnalytics.Shared.Services;

public enum RuntimeType
{
    Server,
    Electron,
    WebAssembly
}

public interface IRuntimeEnvironment
{
    RuntimeType Type { get; }
    bool IsElectron { get; }
    bool IsWebAssembly { get; }
    bool IsServer { get; }
    string GetDisplayName();
}