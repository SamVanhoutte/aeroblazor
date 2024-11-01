namespace AeroBlazor.Security;

public abstract class JwtTokenProvider : ITokenStorageProvider
{
    public abstract Task<IEnumerable<AuthToken>?> GetTokensAsync();
    public abstract Task ClearTokensAsync();
    public abstract Task PersistTokensAsync(IEnumerable<AuthToken> tokens);

}