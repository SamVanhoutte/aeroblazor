using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AeroBlazor.Caching;
using AeroBlazor.Security;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AeroBlazor.Web.Security;

public class AzureB2CTokenManager (IOptions<MicrosoftIdentityOptions> identityOptions, ITokenStorageProvider tokenStorageProvider): IAuthenticationManager
{
    private readonly MicrosoftIdentityOptions identityOptions = identityOptions.Value;
    private InMemoryCache<string, IEnumerable<AuthToken>> tokenCache = new(TimeSpan.FromMinutes(20));
    private InMemoryCache<string, ClaimsIdentity> claimsCache = new(TimeSpan.FromMinutes(1));

    public async Task<bool> IsAuthenticatedAsync()
    {
        var user = await GetUserAsync();

        return user.Identity?.IsAuthenticated ?? false;
    }

    public async Task<string?> SignUpAsync()
    {
        return await GetBearerTokenAsync(true);
    }

    public async Task<string?> GetBearerTokenAsync(bool enforceRefresh = false)
    {
        try
        {
            //if (httpContextAccessor.HttpContext == null) return null;
            if (enforceRefresh)
            {
                var expirationTime = await GetTokenExpirationAsync();
                if (expirationTime < DateTime.UtcNow)
                {
                    await RefreshTokensAsync();
                }
            }

            return await GetTokenAsync("access_token");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<IEnumerable<AuthToken>?> RefreshTokensAsync()
    {
        var refreshToken = await GetRefreshTokenAsync();
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var tenant = identityOptions.Domain!.Split('.').FirstOrDefault();
            var webResponse = await $"https://{tenant}.b2clogin.com"
                .AppendPathSegment(identityOptions.Domain)
                .AppendPathSegment(identityOptions.SignUpSignInPolicyId)
                .AppendPathSegment("oauth2/v2.0/token")
                .AllowAnyHttpStatus()
                .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                .PostUrlEncodedAsync(
                    new
                    {
                        grant_type = "refresh_token",
                        client_id = identityOptions.ClientId,
                        client_secret = identityOptions.ClientSecret,
                        refresh_token = refreshToken
                    });
            if (webResponse.StatusCode < 300)
            {
                var refreshTokenResponse = await webResponse.GetJsonAsync<AuthResponse>();

                var expiresInValue = refreshTokenResponse.expires_in;
                var expiresAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(expiresInValue);
                var tokens = new List<AuthToken>
                {
                    new()
                    {
                        Name = OpenIdConnectParameterNames.IdToken,
                        Value = refreshTokenResponse.id_token
                    },
                    new()
                    {
                        Name = OpenIdConnectParameterNames.AccessToken,
                        Value = refreshTokenResponse.access_token
                    },
                    new()
                    {
                        Name = OpenIdConnectParameterNames.RefreshToken,
                        Value = refreshTokenResponse.refresh_token
                    },
                    new()
                    {
                        Name = "expires_at",
                        Value = expiresAt.ToString("o")
                    }
                };
                await tokenStorageProvider.PersistTokensAsync(tokens);
                return tokens;
            }
            else
            {
                var response = await webResponse.GetStringAsync();
                throw new ApplicationException(
                    $"[{webResponse.StatusCode}]: Error while refreshing tokens: {response}");
            }
        }

        return null;
    }

    private async Task<ClaimsPrincipal> GetUserAsync()
    {
        var identity = await claimsCache.ReadThroughAsync("claims", async(k)=> 
        {
            var jwtToken = await GetBearerTokenAsync();
            // Initialize the JwtSecurityTokenHandler
            var handler = new JwtSecurityTokenHandler();

            // Read the token to get the security token object
            var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

            // Create a ClaimsPrincipal from the JWT token
            return new ClaimsIdentity(jsonToken.Claims, "jwt");
        });

        return new ClaimsPrincipal(identity);
    }

    public async Task<IDictionary<string, string>> GetClaimsAsync()
    {
        var user = await GetUserAsync();
        var claims = user.Claims;
        return claims.ToDictionary(s => s.Type, s => s.Value);
    }

    public async Task<string?> GetClaimAsync(string claimType)
    {
        var user = await GetUserAsync();
        return user.Claims.FirstOrDefault(claim => claim.Type.Equals(claimType))?.Value;
    }

    private async Task<string?> GetRefreshTokenAsync()
    {
        var token = await GetTokenAsync("refresh_token");
        return token;
    }

    private async Task<string?> GetTokenAsync(string tokenName)
    {
        try
        {
            var tokens = await GetTokensAsync();
            return tokens
                ?.FirstOrDefault(token => token.Name.Equals(tokenName, StringComparison.InvariantCultureIgnoreCase))
                ?.Value;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    private async Task<IEnumerable<AuthToken>?> GetTokensAsync()
    {
        return await tokenCache.ReadThroughAsync("tokens",
            async (s) => await tokenStorageProvider.GetTokensAsync());
    }

    public async Task<DateTime> GetTokenExpirationAsync()
    {
        try
        {
            var expiresAtToken = await GetTokenAsync("expires_at");
            if (DateTimeOffset.TryParse(expiresAtToken, out var refreshAt))
            {
                return refreshAt.DateTime;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return DateTime.UtcNow.AddSeconds(-5);
    }

    public async Task SignOutAsync(bool redirect = false)
    {
        await tokenStorageProvider.ClearTokensAsync();
    }
}