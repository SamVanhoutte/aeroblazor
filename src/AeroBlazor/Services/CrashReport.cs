namespace AeroBlazor.Services;

public class CrashReport
{
    public string? UserId { get; set; }
    public string? ErrorMessage { get; set; }
    public string PageUri { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Stacktrace { get; set; }
    public IDictionary<string, string> Context { get; set; }
    public Guid ReportId { get; set; }
    public bool IsApiException { get; set; }
    public int ApiStatusCode { get; set; }
    public string? ApiResponse { get; set; }
}