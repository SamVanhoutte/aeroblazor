using AeroBlazor.Caching;
using Columbae;
using Microsoft.JSInterop;

namespace AeroBlazor.Services.Maps;

public class WebLocationService(IJSRuntime jsRuntime) : ILocationService
{
    private const string CurrentLocationCacheKey = "current";
    private readonly InMemoryCache<string, Polypoint> locationCache = new(TimeSpan.FromSeconds(10));
    private Polypoint? currentLocation ;
    private bool locationRetrieved = false;

    public async Task<Polypoint?> GetLocationAsync()
    {
        return await locationCache.ReadThroughAsync(CurrentLocationCacheKey, async (key) => await QueryLocationAsync());

    }

    public async Task<double> GetDistanceToAsync(Polypoint location)
    {
        var currentLocation = await GetLocationAsync();
        if (currentLocation != null)
        {
            return Columbae.World.Calculator.CalculateDistanceKilometer(location, currentLocation) * 1000;
        }

        return -1;
    }

    public Task<double> GetDistanceToAsync(double longitude, double latitude)
    {
        if (currentLocation != null)
        {
            return Task.FromResult(Columbae.World.Calculator.CalculateDistanceKilometer(new Polypoint(longitude, latitude), currentLocation) * 1000);
        }

        return Task.FromResult<double>(-1);
    }
    
    private async Task<Polypoint?> QueryLocationAsync()
    {
        if (jsRuntime == null)
        {
            return null;
        }
        var coords = await jsRuntime.InvokeAsync<double[]>("getCoords");
        if (coords.Length != 2 || coords[0] == 0 || coords[1] == 0)
        {
            return null;
        }

        return new Polypoint(coords[0], coords[1]);
    }
}