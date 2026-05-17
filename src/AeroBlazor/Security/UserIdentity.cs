namespace AeroBlazor.Security;

public class UserIdentity
{
    public string Email { get; set; } = null!;
    public string Picture { get; set; }= null!;
    public string? IdToken = "";
    public string UserName { get; set; }= null!;
    public string Identifier { get; set; }= null!;
    public string SessionId { get; set; }= null!;
    public IDictionary<string, string> Claims { get; set; } = null!;
    public string? UserId { get; set; }
    public string? AuthenticationProviderId { get; set; }
    public bool IsAdmin { get; set; }
}