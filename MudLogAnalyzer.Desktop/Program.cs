using System.Net;
using ElectronSharp.API;
using ElectronSharp.API.Entities;
using MudBlazor.Services;

Electron.ReadAuth();
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);
builder.Services.AddElectron();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents()
        .AddInteractiveWebAssemblyComponents();



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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<MudLogAnalyzer.Components.App>()
        .AddInteractiveServerRenderMode()
        .AddInteractiveWebAssemblyRenderMode()
        .AddAdditionalAssemblies(typeof(MudLogAnalyzer.Client._Imports).Assembly);
//app.UseHttpsRedirection();
/*app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();*/

//app.MapStaticAssets();
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