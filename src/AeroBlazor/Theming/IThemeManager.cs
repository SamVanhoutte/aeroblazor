using MudBlazor;

namespace AeroBlazor.Theming;

public interface IThemeManager
{
    string? TitleIcon { get; }
    string? SubtitleIcon { get; }
    MudTheme CurrentTheme { get; }
    string PageTitle { get; set; }
}