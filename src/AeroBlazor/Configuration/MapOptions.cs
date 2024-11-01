namespace AeroBlazor.Configuration;

public class MapOptions
{
    public string GoogleMapKey { get; set; }
    public string? GoogleMapStyle { get; set; }
    public string? DefaultMarkerIcon { get; set; } = "_content/AeroBlazor/images/aero-marker-icon.png";
}