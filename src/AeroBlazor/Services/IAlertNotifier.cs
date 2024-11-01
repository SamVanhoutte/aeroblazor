namespace AeroBlazor.Services;

public interface IAlertNotifier
{
    Task ShowAlertAsync(string message, string? title = null, bool isError = false);
}