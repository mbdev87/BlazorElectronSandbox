using LogAnalytics.Shared.Services;

namespace LogAnalytics.Desktop.Services;

public class ElectronRuntimeEnvironment : IRuntimeEnvironment
{
    public RuntimeType Type => RuntimeType.Electron;
    public bool IsElectron => true;
    public bool IsWebAssembly => false;
    public bool IsServer => true; // Electron runs server-side Blazor

    public string GetDisplayName() => "Electron Desktop App";
}