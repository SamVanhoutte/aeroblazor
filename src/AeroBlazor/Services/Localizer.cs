using AeroBlazor.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace AeroBlazor.Services;

public class Localizer(IStringLocalizer localizer, IOptions<AeroBehaviorOptions> aeroBlazorOptions)
{
    private bool LocalizeComponents => aeroBlazorOptions.Value?.LocalizeComponents ?? false;

    public string? this[string? text] =>
        text == null
            ? null
            : LocalizeComponents
                ? localizer[text]
                : text;
}