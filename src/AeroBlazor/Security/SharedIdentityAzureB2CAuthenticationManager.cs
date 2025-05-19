// using System.Security.Claims;
// using AeroBlazor.Caching;
// using Microsoft.Extensions.Options;
// using Flurl;
// using Flurl.Http;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Identity.Web;
// using Microsoft.IdentityModel.Protocols.OpenIdConnect;
//
// namespace AeroBlazor.Security;
//
// public class SharedIdentityAzureB2CAuthenticationManager(
//     IHttpContextAccessor contextAccessor,
//     ITokenStorageProvider tokenStorageProvider,
//     IOptions<MicrosoftIdentityOptions> identityOptions)
//     : AzureB2CTokenManager
// {
//     private readonly MicrosoftIdentityOptions identityOptions = identityOptions.Value;
//     private InMemoryCache<string, IEnumerable<AuthToken>> tokenCache = new(TimeSpan.FromMinutes(20));
//
//     //
//     // public async Task<string?> GetBearerTokenAsync(bool enforceRefresh = false)
//     // {
//     //     try
//     //     {
//     //         //if (httpContextAccessor.HttpContext == null) return null;
//     //         if (enforceRefresh)
//     //         {
//     //             var expirationTime = await GetTokenExpirationAsync();
//     //             if (expirationTime < DateTime.UtcNow)
//     //             {
//     //                 await RefreshTokensAsync();
//     //             }
//     //         }
//     //
//     //         return await GetTokenAsync("access_token");
//     //     }
//     //     catch (Exception e)
//     //     {
//     //         return null;
//     //     }
//     // }
//     //
//     //
//     // public async Task<IEnumerable<AuthToken>?> RefreshTokensAsync()
//     // {
//     //     var refreshToken = await GetRefreshTokenAsync();
//     //     if (!string.IsNullOrEmpty(refreshToken))
//     //     {
//     //         var tenant = identityOptions.Domain!.Split('.').FirstOrDefault();
//     //         var webResponse = await $"https://{tenant}.b2clogin.com"
//     //             .AppendPathSegment(identityOptions.Domain)
//     //             .AppendPathSegment(identityOptions.SignUpSignInPolicyId)
//     //             .AppendPathSegment("oauth2/v2.0/token")
//     //             .AllowAnyHttpStatus()
//     //             .WithHeader("Content-Type", "application/x-www-form-urlencoded")
//     //             .PostUrlEncodedAsync(
//     //                 new
//     //                 {
//     //                     grant_type = "refresh_token",
//     //                     client_id = identityOptions.ClientId,
//     //                     client_secret = identityOptions.ClientSecret,
//     //                     refresh_token = refreshToken
//     //                 });
//     //         if (webResponse.StatusCode < 300)
//     //         {
//     //             var refreshTokenResponse = await webResponse.GetJsonAsync<AuthResponse>();
//     //
//     //             var expiresInValue = refreshTokenResponse.expires_in;
//     //             var expiresAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(expiresInValue);
//     //             var tokens = new List<AuthToken>
//     //             {
//     //                 new()
//     //                 {
//     //                     Name = OpenIdConnectParameterNames.IdToken,
//     //                     Value = refreshTokenResponse.id_token
//     //                 },
//     //                 new()
//     //                 {
//     //                     Name = OpenIdConnectParameterNames.AccessToken,
//     //                     Value = refreshTokenResponse.access_token
//     //                 },
//     //                 new()
//     //                 {
//     //                     Name = OpenIdConnectParameterNames.RefreshToken,
//     //                     Value = refreshTokenResponse.refresh_token
//     //                 },
//     //                 new()
//     //                 {
//     //                     Name = "expires_at",
//     //                     Value = expiresAt.ToString("o")
//     //                 }
//     //             };
//     //             await tokenStorageProvider.PersistTokensAsync(tokens);
//     //             return tokens;
//     //         }
//     //         else
//     //         {
//     //             var response = await webResponse.GetStringAsync();
//     //             throw new ApplicationException(
//     //                 $"[{webResponse.StatusCode}]: Error while refreshing tokens: {response}");
//     //         }
//     //     }
//     //
//     //     return null;
//     // }
//     //
//     // private async Task<ClaimsPrincipal> GetUserAsync()
//     // {
//     //     return new ClaimsPrincipal();
//     // }
//     //
//     // public async Task<IDictionary<string, string>> GetClaimsAsync()
//     // {
//     //     var user = await GetUserAsync();
//     //     var claims = user.Claims;
//     //     return claims.ToDictionary(s => s.Type, s => s.Value);
//     // }
//     //
//     // private async Task<string?> GetRefreshTokenAsync()
//     // {
//     //     if (contextAccessor.HttpContext == null) return null;
//     //     var token = await GetTokenAsync("refresh_token");
//     //     return token;
//     // }
//     //
//     // private async Task<string?> GetTokenAsync(string tokenName)
//     // {
//     //     try
//     //     {
//     //         var tokens = await GetTokensAsync();
//     //         return tokens
//     //             ?.FirstOrDefault(token => token.Name.Equals(tokenName, StringComparison.InvariantCultureIgnoreCase))
//     //             ?.Value;
//     //     }
//     //     catch (Exception e)
//     //     {
//     //         return null;
//     //     }
//     // }
//     //
//     // private async Task<IEnumerable<AuthToken>?> GetTokensAsync()
//     // {
//     //     return await tokenCache.ReadThroughAsync("tokens",
//     //         async(s) => await tokenStorageProvider.GetTokensAsync());
//     //     
//     // }
//     //
//     // public async Task<DateTime> GetTokenExpirationAsync()
//     // {
//     //     try
//     //     {
//     //         var expiresAtToken = await GetTokenAsync("expires_at");
//     //         if (DateTimeOffset.TryParse(expiresAtToken, out var refreshAt))
//     //         {
//     //             return refreshAt.DateTime;
//     //         }
//     //     }
//     //     catch (Exception e)
//     //     {
//     //         Console.WriteLine(e);
//     //     }
//     //
//     //     return DateTime.UtcNow.AddSeconds(-5);
//     // }
//     //
//     // public async Task SignOutAsync(bool redirect = false)
//     // {
//     //     await tokenStorageProvider.ClearTokensAsync();
//     // }
//
//
// }