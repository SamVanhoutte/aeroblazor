namespace AeroBlazor.Security;

public interface IAuthenticationManager
{
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetBearerTokenAsync(bool enforceRefresh = false);
    Task<IEnumerable<AuthToken>?> RefreshTokensAsync();
    Task<IDictionary<string, string>> GetClaimsAsync();
    Task<string?> GetClaimAsync(string claimType);
    Task<DateTime> GetTokenExpirationAsync();
    Task SignOutAsync(bool redirect = false);
}