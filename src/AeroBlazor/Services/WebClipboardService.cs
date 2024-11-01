using Microsoft.JSInterop;

namespace AeroBlazor.Services;

public class WebClipboardService(IJSRuntime jsRuntime) : IClipboardService
{
    public async Task<bool> CopyToClipboardAsync(string value)
    {
        await jsRuntime.InvokeVoidAsync("clipboardCopy.copyText", value);
        return true;
    }
}
