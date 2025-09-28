using LogAnalytics.Shared;

namespace LogAnalytics.Client.Services;

public class WasmEnvironmentInfoService : IEnvironmentInfoService
{
    public string GetMode() => "WASM";
}
