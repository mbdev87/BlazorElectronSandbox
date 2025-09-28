using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;


namespace LogAnalytics.Shared;

public static class RenderModeHelper
{
    // For web scenarios where we want Auto mode with prerendering
    public static IComponentRenderMode GetWebRenderMode()
        => new InteractiveAutoRenderMode(prerender: true);

    // For desktop scenarios where render modes don't apply
    public static IComponentRenderMode? GetDesktopRenderMode()
        => null;
}