using System.Net;
using ElectronSharp.API;
using ElectronSharp.API.Entities;
using MudBlazor.Services;

//var isMaximized = false;
Electron.ReadAuth();
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);
builder.Services.AddElectron();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add platform detection service
builder.Services.AddScoped<MudLogAnalyzer.Client.Services.IPlatformService>(sp =>
    new MudLogAnalyzer.Client.Services.PlatformService { IsElectron = true });

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
                Width = 800,
                Height = 600,
                WebPreferences = new WebPreferences
                {
                                NodeIntegration = false,
                                ContextIsolation = true,
                                WebSecurity = true,
                                Preload = Path.Join(Directory.GetCurrentDirectory(), "wwwroot", "electron-preload.js"),
                                ExperimentalCanvasFeatures = true,
                        
                                //Transparent = true
                }, 
                ThickFrame = true,
                Frame = true,
                DarkTheme = true,
                TitleBarStyle = TitleBarStyle.hidden,
                TitleBarOverlay = new TitleBarOverlayConfig
                {
                    color = "#1b1a19",
                    symbolColor = "#ffffff"
                },
                
                HasShadow = true,
                AutoHideMenuBar = true,
                Center = true,

});

// If you want to experiment with acrylic 
// set above                 Transparent = true,


/*Electron.IpcMain.On("window-control", (action) =>
{
    var command = action?.ToString();
    switch (command)
    {
        case "minimize":
            browserWindow.Minimize();
            break;
        case "maximize":
            Console.WriteLine($"{nameof(isMaximized)} value: {isMaximized}");
            if (isMaximized)
            {
                browserWindow.Unmaximize();
                isMaximized = false;
            }
            else
            {
                browserWindow.Maximize();
                isMaximized = true;
            }
            break;
        case "close":
            browserWindow.Close();
            break;
    }
});

if (OperatingSystem.IsWindows())
{
    Electron.NativeTheme.ThemeSource = "dark";
}
else if (OperatingSystem.IsMacOS())
{
    browserWindow.SetVibrancy(Vibrancy.underWindow);
}


browserWindow.OnMaximize += () => isMaximized = true;
browserWindow.OnUnmaximize += () => isMaximized = false;*/

browserWindow.LoadURL("http://localhost:5000");

await app.WaitForShutdownAsync();