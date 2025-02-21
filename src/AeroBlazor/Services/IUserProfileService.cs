using AeroBlazor.Security;

namespace AeroBlazor.Services;

public interface IUserProfileService
{
    UserIdentity Identity { get; }
    event EventHandler AuthenticationStateChanged;
    ValueTask<UserIdentity> GetIdentityInfoAsync();

}