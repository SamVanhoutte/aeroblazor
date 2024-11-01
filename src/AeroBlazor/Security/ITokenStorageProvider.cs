namespace AeroBlazor.Security;

public interface ITokenStorageProvider
{
    Task<IEnumerable<AuthToken>?> GetTokensAsync();
    Task ClearTokensAsync();
    Task PersistTokensAsync(IEnumerable<AuthToken> tokens);
}