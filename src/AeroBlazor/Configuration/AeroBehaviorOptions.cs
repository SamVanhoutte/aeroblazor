namespace AeroBlazor.Configuration;

public class AeroBehaviorOptions
{
    public string? CustomLoaderImageUrl { get; set; }
    public static AeroBehaviorOptions Default => new AeroBehaviorOptions();
}