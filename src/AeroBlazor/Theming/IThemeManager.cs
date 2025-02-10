using MudBlazor;

namespace AeroBlazor.Theming;

public interface IThemeManager
{
    string? SubtitleIcon { get; }
    MudTheme CurrentTheme { get; }
    string PageTitle { get; set; }
}