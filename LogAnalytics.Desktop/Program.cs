using System.Net;
using System.IO;
using ElectronSharp.API;
using ElectronSharp.API.Entities;
using LogAnalytics.Client.Services;
using LogAnalytics.Desktop.Services;
using LogAnalytics.Server.Services;
using LogAnalytics.Shared;
using LogAnalytics.Shared.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

Global.DefaultRenderMode = new InteractiveServerRenderMode(prerender: true);

Electron.ReadAuth();
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);
builder.Services.AddElectron();


builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

builder.Services.AddScoped<IEnvironmentInfoService, ServerEnvironmentInfoService>();
builder.Services.AddSingleton<IUserSettingsService, FileUserSettingsService>();
builder.Services.AddSingleton<IThemeService, ServerThemeService>();
builder.Services.AddSingleton<IRuntimeEnvironment, ElectronRuntimeEnvironment>();
builder.Services.AddFluentUIComponents();

builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    serverOptions.Listen(IPAddress.Loopback, 5000);
});

builder.Services.AddScoped(sp =>
{
    var httpContext = sp.GetService<IHttpContextAccessor>()?.HttpContext;
    var baseAddress = httpContext != null
            ? $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/"
            : "http://localhost:5000/";
    return new HttpClient { BaseAddress = new Uri(baseAddress) };
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();


app.MapRazorComponents<LogAnalytics.Server.Components.App>()
        .AddInteractiveServerRenderMode()
        .AddAdditionalAssemblies(typeof(LogAnalytics.Client._Imports).Assembly);
app.MapStaticAssets();
await app.StartAsync();

var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
{
        Width = 1152,
        Height = 940,
        WebPreferences = new WebPreferences
        {
                NodeIntegration = false,   // no Node globals polluting the DOM
                ContextIsolation = true,   // sandbox, safer for preload bridging
                WebSecurity = true,
                Preload = Path.Join(Directory.GetCurrentDirectory(), "wwwroot", "electron-preload.js")
        }
});

browserWindow.LoadURL("http://localhost:5000");

await app.WaitForShutdownAsync();