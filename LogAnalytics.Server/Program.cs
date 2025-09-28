using LogAnalytics.Client.Services;
using LogAnalytics.Server.Components;
using LogAnalytics.Server.Services;
using LogAnalytics.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
Global.DefaultRenderMode = new InteractiveAutoRenderMode(prerender: true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents()
        .AddInteractiveWebAssemblyComponents();
builder.Services.AddScoped<IEnvironmentInfoService, ServerEnvironmentInfoService>();

builder.Services.AddFluentUIComponents();
builder.Services.AddScoped(sp =>
{
    var httpContext = sp.GetService<IHttpContextAccessor>()?.HttpContext;
    var baseAddress = httpContext != null
        ? $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/"
        : "https://localhost:5000/";
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

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAntiforgery();


app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode()
        .AddInteractiveWebAssemblyRenderMode()
        .AddAdditionalAssemblies(typeof(LogAnalytics.Client._Imports).Assembly);

app.Run();