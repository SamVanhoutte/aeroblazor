using Columbae;

namespace AeroBlazor.Services.Maps;

public interface ILocationService
{
    Task<Polypoint?> GetLocationAsync();
    Task<double> GetDistanceToAsync(Polypoint location);
    Task<double> GetDistanceToAsync(double longitude, double latitude);
}