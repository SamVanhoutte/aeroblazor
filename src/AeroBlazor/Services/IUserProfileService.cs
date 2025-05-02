using AeroBlazor.Security;

namespace AeroBlazor.Services;

public interface IUserProfileService
{
    bool IsAuthenticated { get; }
    UserIdentity Identity { get; }
    event EventHandler AuthenticationStateChanged;
    ValueTask<UserIdentity> GetIdentityInfoAsync();

}