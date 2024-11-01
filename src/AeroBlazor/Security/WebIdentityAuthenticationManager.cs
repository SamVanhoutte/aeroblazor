// using System.Security.Claims;
// using Microsoft.Extensions.Options;
// using Newtonsoft.Json;
// using Flurl;
// using Flurl.Http;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Components.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Identity.Web;
// using Microsoft.IdentityModel.Protocols.OpenIdConnect;
//
// namespace AeroBlazor.Security;
//
// public class WebIdentityAuthenticationManager : IAuthenticationManager
// {
//     private readonly IHttpContextAccessor httpContextAccessor;
//     private readonly AuthenticationStateProvider authenticationStateProvider;
//     private readonly ITokenStorageProvider authTokenStorageProvider;
//     private readonly MicrosoftIdentityOptions identityOptions;
//
//     private IDictionary<string, string> tokens = new Dictionary<string, string>();
//
//     public WebIdentityAuthenticationManager(IHttpContextAccessor httpContextAccessor,
//         AuthenticationStateProvider authenticationStateProvider,
//         ITokenStorageProvider authTokenStorageProvider,
//         IOptions<MicrosoftIdentityOptions> identityOptions)
//     {
//         this.httpContextAccessor = httpContextAccessor;
//         this.authenticationStateProvider = authenticationStateProvider;
//         this.authTokenStorageProvider = authTokenStorageProvider;
//         this.identityOptions = identityOptions.Value;
//     }
//
//     public Task<ClaimsPrincipal?> GetClaimsPrincipalAsync()
//     {
//         return Task.FromResult(httpContextAccessor.HttpContext?.User);
//     }
//
//     public async Task<bool> IsAuthenticatedAsync()
//     {
//         var user = await GetUserAsync();
//
//         return user.Identity?.IsAuthenticated ?? false;
//     }
//     public async Task<string?> SignUpAsync()
//     {
//         return await GetBearerTokenAsync(true);
//     }
//     
//     public async Task<string?> GetBearerTokenAsync(bool enforceRefresh = false)
//     {
//         try
//         {
//             //if (httpContextAccessor.HttpContext == null) return null;
//             if (enforceRefresh)
//             {
//                 var expirationTime = await GetTokenExpirationAsync();
//                 if (expirationTime < DateTime.UtcNow)
//                 {
//                     await RefreshTokensAsync();
//                 }
//             }
//
//             return await GetTokenAsync("access_token");
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             return null;
//         }
//     }
//
//     public async Task<IEnumerable<AuthToken>?> RefreshTokensAsync()
//     {
//         var refreshToken = await GetRefreshTokenAsync();
//         if (!string.IsNullOrEmpty(refreshToken))
//         {
//             var tenant = identityOptions.Domain!.Split('.').FirstOrDefault();
//             var webResponse = await $"https://{tenant}.b2clogin.com"
//                 .AppendPathSegment(identityOptions.Domain)
//                 .AppendPathSegment(identityOptions.SignUpSignInPolicyId)
//                 .AppendPathSegment("oauth2/v2.0/token")
//                 .AllowAnyHttpStatus()
//                 .WithHeader("Content-Type", "application/x-www-form-urlencoded")
//                 .PostUrlEncodedAsync(
//                     new
//                     {
//                         grant_type = "refresh_token",
//                         client_id = identityOptions.ClientId,
//                         client_secret = identityOptions.ClientSecret,
//                         refresh_token = refreshToken
//                     });
//             if (webResponse.StatusCode < 300)
//             {
//                 var refreshTokenResponse = await webResponse.GetJsonAsync<AuthResponse>();
//
//                 var expiresInValue = refreshTokenResponse.ExpiresIn;
//                 var secondsToExpire = 0;
//                     
//                 
//                 var expiresAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(secondsToExpire);
//                 var tokens = new List<AuthToken>
//                 {
//                     new()
//                     {
//                         Name = OpenIdConnectParameterNames.IdToken,
//                         Value = refreshTokenResponse.IdToken
//                     },
//                     new()
//                     {
//                         Name = OpenIdConnectParameterNames.AccessToken,
//                         Value = refreshTokenResponse.AccessToken
//                     },
//                     new()
//                     {
//                         Name = OpenIdConnectParameterNames.RefreshToken,
//                         Value = refreshTokenResponse.RefreshToken
//                     },
//                     new()
//                     {
//                         Name = "expires_at",
//                         Value = expiresAt.ToString("o")
//                     }
//                 };
//                 await authTokenStorageProvider.PersistTokensAsync(tokens);
//                 return tokens;
//             }
//             else
//             {
//                 var response = await webResponse.GetStringAsync();
//                 throw new ApplicationException(
//                     $"[{webResponse.StatusCode}]: Error while refreshing tokens: {response}");
//             }
//         }
//
//         return null;
//     }
//
//     private async Task<ClaimsPrincipal> GetUserAsync()
//     {
//         var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
//         var user = authState.User;
//         return user;
//     }
//
//     public async Task<IDictionary<string, string>> GetClaimsAsync()
//     {
//         var user = await GetUserAsync();
//         var claims = user.Claims;
//         return claims.ToDictionary(s => s.Type, s => s.Value);
//     }
//
//     private async Task<string?> GetRefreshTokenAsync()
//     {
//         if (httpContextAccessor.HttpContext == null) return null;
//         var token = await GetTokenAsync("refresh_token");
//         return token;
//     }
//
//     private async Task<string?> GetTokenAsync(string tokenName)
//     {
//         try
//         {
//             var tokens = await authTokenStorageProvider.GetTokensAsync();
//             string? tokenValue = null;
//             if (httpContextAccessor.HttpContext != null)
//             {
//                 tokenValue = await httpContextAccessor.HttpContext.GetTokenAsync(tokenName);
//
//                 var persistedToken =
//                     tokens?.FirstOrDefault(t => t.Name.Equals(tokenName, StringComparison.InvariantCultureIgnoreCase));
//                 if (persistedToken != null) tokenValue = persistedToken.Value;
//             }
//
//             return tokenValue;
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             return null;
//         }
//     }
//
//     public async Task<DateTime> GetTokenExpirationAsync()
//     {
//         try
//         {
//             if (httpContextAccessor.HttpContext != null)
//             {
//                 var expiresAtToken = await GetTokenAsync("expires_at");
//                 if (DateTimeOffset.TryParse(expiresAtToken, out var refreshAt))
//                 {
//                     return refreshAt.DateTime;
//                 }
//             }
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//         }
//
//         return DateTime.UtcNow.AddSeconds(-5);
//     }
//     
//     public async Task SignOutAsync(bool redirect = false)
//     {
//         await authTokenStorageProvider.ClearTokensAsync();
//     }
//
//     private struct AuthResponse
//     {
//         [JsonProperty("expires_in")] public int ExpiresIn { get; set; }
//         [JsonProperty("access_token")] public string? AccessToken { get; set; }
//         [JsonProperty("id_token")] public string? IdToken { get; set; }
//         [JsonProperty("refresh_token")] public string? RefreshToken { get; set; }
//     }
// }