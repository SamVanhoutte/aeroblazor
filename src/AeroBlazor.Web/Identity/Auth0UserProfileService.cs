using AeroBlazor.Configuration;
using AeroBlazor.Security;
using AeroBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace AeroBlazor.Web.Identity;

public class Auth0UserProfileService : IUserProfileService
{
    private readonly AeroStartupOptions aeroOptions;

    public Auth0UserProfileService(AuthenticationStateProvider authenticationStateProvider,
        AeroStartupOptions aeroOptions)
    {
        this.authenticationStateProvider = authenticationStateProvider;
        this.aeroOptions = aeroOptions;
        authenticationStateProvider.AuthenticationStateChanged +=
            AuthenticationStateProvider_AuthenticationStateChanged;
    }

    public bool IsAuthenticated => Identity != null;

    public UserIdentity? Identity => identityInformation ??= GetIdentityInfoAsync().GetAwaiter().GetResult();

    public bool IsAdmin => Identity?.IsAdmin ?? false;

    public event EventHandler? AuthenticationStateChanged;

    private void AuthenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
    {
        var authState = task.Result;
        UpdateIdentityInfo(authState);
        AuthenticationStateChanged?.Invoke(this, new EventArgs());
    }

    public async ValueTask<UserIdentity> GetIdentityInfoAsync()
    {
        if (identityInformation == default)
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            UpdateIdentityInfo(authState);
        }

        return identityInformation;
    }

    private void UpdateIdentityInfo(AuthenticationState authState)
    {
        if (authState.User?.Identity?.IsAuthenticated ?? false)
        {
            var user = authState.User;
            identityInformation = new UserIdentity();
            identityInformation.UserName = user.Identity.Name ?? string.Empty;
            identityInformation.Email = user.Claims
                .Where(c => c.Type.Equals(System.Security.Claims.ClaimTypes.Email))
                .Select(c => c.Value)
                .FirstOrDefault() ?? string.Empty;
            identityInformation.AuthenticationProviderId = user.Claims
                .Where(c => c.Type.Equals(System.Security.Claims.ClaimTypes.NameIdentifier))
                .Select(c => c.Value)
                .FirstOrDefault() ?? null;
            identityInformation.Picture = user.Claims
                .Where(c => c.Type.Equals("picture"))
                .Select(c => c.Value)
                .FirstOrDefault() ?? string.Empty;
            identityInformation.IsAdmin = user.Claims
                .Where(c => c.Type.Equals(System.Security.Claims.ClaimTypes.Role))
                .Select(c => c.Value)
                .FirstOrDefault()?.Equals("Admin", StringComparison.InvariantCultureIgnoreCase) ?? false;

            if (!string.IsNullOrEmpty(aeroOptions.UserIdClaim))
            {
                identityInformation.UserId = user.Claims
                    .Where(c => c.Type.Equals(aeroOptions.UserIdClaim))
                    .Select(c => c.Value)
                    .FirstOrDefault() ?? null;
            }

            identityInformation.Claims = user.Claims
                .Select(claim => new KeyValuePair<string, string>(claim.Type, claim.Value)).ToDictionary();
        }
        else
        {
            this.identityInformation = null;
        }
    }

    private readonly AuthenticationStateProvider authenticationStateProvider;
    private UserIdentity? identityInformation;
}