namespace AeroBlazor.Security;

public class AuthToken
{
    public AuthToken()
    {
        
    }
    public AuthToken(string name, string value)
    {
        Name = name;
        Value = value;
    }
    public string Name { get; set; }
    public string Value { get; set; }
}