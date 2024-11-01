using Columbae;

namespace AeroBlazor.Services.Maps;

public class MapMarker
{
    public string Id { get; set; }
    public string Label { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Icon { get; set; }
    public string? Link { get; set; }
    public bool LocationEnabled => Latitude != 0D && Longitude != 0D;
    public Polypoint Point => new Polypoint(Longitude, Latitude);
}