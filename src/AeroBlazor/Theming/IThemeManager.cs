using MudBlazor;

namespace AeroBlazor.Theming;

public interface IThemeManager
{
    MudTheme CurrentTheme { get; }
    string PageTitle { get; set; }
}