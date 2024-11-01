using MudBlazor;

namespace AeroBlazor.Services;

public class WebAlertNotifier(ISnackbar provider) : IAlertNotifier
{
    public Task ShowAlertAsync(string message, string? title = null, bool isError = false)
    {
        provider.Add(message, isError ? Severity.Error : Severity.Normal);
        return Task.CompletedTask;
    }
}