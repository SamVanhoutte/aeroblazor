namespace AeroBlazor.Services;

public interface ICrashReportHandler
{
    Task<ExceptionHandlingResult> HandleExceptionAsync(Exception ex);
    Task<IEnumerable<CrashReport>> GetCrashReportsAsync(DateTime? fromDate = null);
    Task<CrashReport> GetCrashReportAsync(Guid reportId);
    Task ClearAllAsync();
}