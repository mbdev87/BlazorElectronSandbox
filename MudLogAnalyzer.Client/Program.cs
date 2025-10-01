using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;
using MudLogAnalyzer.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

var isElectron = builder.HostEnvironment.BaseAddress.StartsWith("http://localhost:5000");

builder.Services.AddScoped<IPlatformService>(sp =>
    new PlatformService { IsElectron = isElectron });

await builder.Build().RunAsync();
