using Microsoft.JSInterop;
using MudBlazor;
using System.Text.Json;

namespace MudLogAnalyzer.Client.Services;

public record ColorTint(string Name, string HexValue);

public record ThemeSettings
{
    public ColorTint? UxTint { get; init; }
    public ColorTint? ContentTint { get; init; }
    public double UxIntensity { get; init; } = 0.10;
    public double ContentIntensity { get; init; } = 0.05;
}

public interface IThemeService
{
    ThemeSettings Settings { get; }
    Task UpdateSettingsAsync(ThemeSettings settings);
    Task<ThemeSettings> GetSettingsAsync();
    event Action? OnThemeChanged;
    MudTheme GenerateTheme();
    List<ColorTint> AvailableTints { get; }
}

public class ThemeService : IThemeService
{
    private readonly IJSRuntime? _jsRuntime;
    private ThemeSettings _settings;

    public event Action? OnThemeChanged;

    public List<ColorTint> AvailableTints { get; } = new()
    {
        new ColorTint("None", "#000000"),
        new ColorTint("Slate", "#64748b"),
        new ColorTint("Gray", "#6b7280"),
        new ColorTint("Zinc", "#71717a"),
        new ColorTint("Neutral", "#737373"),
        new ColorTint("Stone", "#78716c"),
        new ColorTint("Red", "#991b1b"),
        new ColorTint("Orange", "#c2410c"),
        new ColorTint("Amber", "#d97706"),
        new ColorTint("Yellow", "#ca8a04"),
        new ColorTint("Lime", "#4d7c0f"),
        new ColorTint("Green", "#15803d"),
        new ColorTint("Emerald", "#047857"),
        new ColorTint("Teal", "#0f766e"),
        new ColorTint("Cyan", "#0e7490"),
        new ColorTint("Sky", "#0369a1"),
        new ColorTint("Blue", "#1e40af"),
        new ColorTint("Indigo", "#4338ca"),
        new ColorTint("Violet", "#6d28d9"),
        new ColorTint("Purple", "#7e22ce"),
        new ColorTint("Fuchsia", "#a21caf"),
        new ColorTint("Pink", "#be185d"),
        new ColorTint("Rose", "#be123c"),
    };

    public ThemeSettings Settings => _settings;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _settings = new ThemeSettings();
    }

    public ThemeService()
    {
        _settings = new ThemeSettings();
    }

    public async Task UpdateSettingsAsync(ThemeSettings settings)
    {
        _settings = settings;
        if (_jsRuntime != null)
        {
            var json = JsonSerializer.Serialize(new
            {
                UxTintHex = settings.UxTint?.HexValue,
                ContentTintHex = settings.ContentTint?.HexValue,
                settings.UxIntensity,
                settings.ContentIntensity
            });
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "theme-settings", json);
        }
        OnThemeChanged?.Invoke();
    }

    public async Task<ThemeSettings> GetSettingsAsync()
    {
        if (_jsRuntime != null)
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "theme-settings");
                if (!string.IsNullOrEmpty(json))
                {
                    var stored = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                    if (stored != null)
                    {
                        ColorTint? uxTint = null;
                        ColorTint? contentTint = null;

                        if (stored.TryGetValue("UxTintHex", out var uxHex) && uxHex.ValueKind == JsonValueKind.String)
                        {
                            uxTint = AvailableTints.FirstOrDefault(t => t.HexValue == uxHex.GetString());
                        }

                        if (stored.TryGetValue("ContentTintHex", out var contentHex) && contentHex.ValueKind == JsonValueKind.String)
                        {
                            contentTint = AvailableTints.FirstOrDefault(t => t.HexValue == contentHex.GetString());
                        }

                        _settings = new ThemeSettings
                        {
                            UxTint = uxTint,
                            ContentTint = contentTint,
                            UxIntensity = stored.TryGetValue("UxIntensity", out var uxInt) ? uxInt.GetDouble() : 0.10,
                            ContentIntensity = stored.TryGetValue("ContentIntensity", out var contentInt) ? contentInt.GetDouble() : 0.05
                        };
                        return _settings;
                    }
                }
            }
            catch
            {
            }
        }

        return _settings;
    }

    public MudTheme GenerateTheme()
    {
        var uxTint = _settings.UxTint?.Name != "None" ? _settings.UxTint?.HexValue : null;
        var contentTint = _settings.ContentTint?.Name != "None" ? _settings.ContentTint?.HexValue : null;

        var primaryColor = uxTint ?? contentTint ?? "#0078d4";
        var primaryDark = LightenColor(primaryColor, 0.2);

        return new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = primaryColor,
                Secondary = LightenColor(primaryColor, 0.1),
                Black = "#201f1e",
                AppbarText = "#323130",
                AppbarBackground = "#ffffff",
                DrawerBackground = "#ffffff",
                Background = contentTint != null ? ApplyTint("#f3f2f1", contentTint, _settings.ContentIntensity) : "#f3f2f1",
                Surface = contentTint != null ? ApplyTint("#ffffff", contentTint, _settings.ContentIntensity) : "#ffffff",
                TextPrimary = "#201f1e",
                TextSecondary = "#605e5c",
                GrayLight = "#edebe9",
                GrayLighter = "#faf9f8",
                Info = primaryColor,
                Success = "#107c10",
                Warning = "#ffb900",
                Error = "#d13438",
            },
            PaletteDark = new PaletteDark
            {
                Primary = primaryDark,
                Secondary = LightenColor(primaryDark, 0.1),
                Surface = contentTint != null ? ApplyTint("#1b1a19", contentTint, _settings.ContentIntensity) : "#1b1a19",
                Background = contentTint != null ? ApplyTint("#11100f", contentTint, _settings.ContentIntensity) : "#11100f",
                BackgroundGray = contentTint != null ? ApplyTint("#201f1e", contentTint, _settings.ContentIntensity) : "#201f1e",
                AppbarText = "#f3f2f1",
                AppbarBackground = uxTint != null ? ApplyTint("#1b1a19", uxTint, _settings.UxIntensity) : "#1b1a19",
                DrawerBackground = uxTint != null ? ApplyTint("#1b1a19", uxTint, _settings.UxIntensity) : "#1b1a19",
                ActionDefault = "#a19f9d",
                ActionDisabled = "#8a88864d",
                ActionDisabledBackground = "#3231304d",
                TextPrimary = "#ffffff",
                TextSecondary = "#d2d0ce",
                TextDisabled = "#ffffff66",
                DrawerIcon = "#d2d0ce",
                DrawerText = "#d2d0ce",
                GrayLight = "#323130",
                GrayLighter = "#252423",
                Info = primaryDark,
                Success = "#107c10",
                Warning = "#fce100",
                Error = "#d13438",
                LinesDefault = "#3b3a39",
                TableLines = "#3b3a39",
                Divider = "#323130",
                OverlayLight = "#201f1e80",
            },
            LayoutProperties = new LayoutProperties()
        };
    }

    private string ApplyTint(string baseColor, string tintColor, double intensity)
    {
        var baseRgb = HexToRgb(baseColor);
        var tintRgb = HexToRgb(tintColor);

        var r = (int)(baseRgb.r + (tintRgb.r - baseRgb.r) * intensity);
        var g = (int)(baseRgb.g + (tintRgb.g - baseRgb.g) * intensity);
        var b = (int)(baseRgb.b + (tintRgb.b - baseRgb.b) * intensity);

        return RgbToHex(r, g, b);
    }

    private (int r, int g, int b) HexToRgb(string hex)
    {
        hex = hex.TrimStart('#');
        return (
            Convert.ToInt32(hex.Substring(0, 2), 16),
            Convert.ToInt32(hex.Substring(2, 2), 16),
            Convert.ToInt32(hex.Substring(4, 2), 16)
        );
    }

    private string RgbToHex(int r, int g, int b)
    {
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    private string LightenColor(string hexColor, double amount)
    {
        var rgb = HexToRgb(hexColor);

        var r = Math.Min(255, (int)(rgb.r + (255 - rgb.r) * amount));
        var g = Math.Min(255, (int)(rgb.g + (255 - rgb.g) * amount));
        var b = Math.Min(255, (int)(rgb.b + (255 - rgb.b) * amount));

        return RgbToHex(r, g, b);
    }
}
