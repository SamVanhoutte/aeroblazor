namespace AeroBlazor.Configuration;

public class AeroBehaviorOptions
{
    public string? CustomLoaderImageUrl { get; set; }
    public bool LocalizeComponents { get; set; } = true;
    public static AeroBehaviorOptions Default => new AeroBehaviorOptions();
}