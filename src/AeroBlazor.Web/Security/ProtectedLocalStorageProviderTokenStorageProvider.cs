using AeroBlazor.Security;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace AeroBlazor.Web.Security;

public class ProtectedLocalStorageProviderTokenStorageProvider(ProtectedLocalStorage storage) : JwtTokenProvider
{
    private const string TokenStorageName = "AuthTokens";

    public override async Task<IEnumerable<AuthToken>?> GetTokensAsync()
    {
        try
        {
            var result = await storage.GetAsync<IEnumerable<AuthToken>>(TokenStorageName);
            if (result.Success) return result.Value;
            return new List<AuthToken>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new List<AuthToken>();
        }
    }

    public override async Task ClearTokensAsync()
    {
        await storage.DeleteAsync(TokenStorageName);
    }

    public override async Task PersistTokensAsync(IEnumerable<AuthToken> tokens)
    {
        await storage.SetAsync(TokenStorageName, tokens);
    }
}