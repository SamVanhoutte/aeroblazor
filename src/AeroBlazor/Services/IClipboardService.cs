namespace AeroBlazor.Services;

public interface IClipboardService
{
    Task<bool> CopyToClipboardAsync(string? value);
}