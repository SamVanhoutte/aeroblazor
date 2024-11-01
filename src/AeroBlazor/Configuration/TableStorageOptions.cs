namespace AeroBlazor.Configuration;

public class TableStorageOptions
{
    public string StorageAccount { get; set; }
    public string StorageAccountKey { get; set; }
    public string AuthTableName { get; set; }
    public string ClientName { get; set; }
    public Uri TableEndpointUri => new Uri($"https://{StorageAccount}.table.core.windows.net/");
}