using System.Reflection;
using System.Text;
using Columbae;
using GoogleMapsComponents;
using GoogleMapsComponents.Maps;
using GoogleMapsComponents.Maps.Extension;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MapOptions = AeroBlazor.Configuration.MapOptions;

namespace AeroBlazor.Services.Maps;

public class AeroMapService(
    TranslatorService service,
    IOptions<MapOptions> mapSettings)
{
    private readonly MapOptions mapOptions = mapSettings.Value;
    private List<Marker>? _markers = new List<Marker>();
    private MarkerList? markerList;
    protected MarkerClustering? MarkerCluster;

    public async Task ClearMarkersAsync()
    {
        if (_markers != null)
        {
            var tasks = new List<Task> { };
            tasks.AddRange(
                _markers.Select(marker => marker.SetMap(null)));
            await Task.WhenAll(tasks);
        }

        if (MarkerCluster != null)
        {
            await MarkerCluster.ClearMarkers();
            await MarkerCluster.SetMap(null);
        }

        _markers = new List<Marker>();
    }

    protected MarkerOptions GetMarker(MapMarker marker, GoogleMap map)
    {
        var options = new MarkerOptions()
        {
            Position = new LatLngLiteral(marker.Latitude, marker.Longitude),
            Map = map.InteropObject,
            Clickable = true,
            Title = marker.Label,
            Visible = true
        };
        var markerIcon = marker.Icon ?? mapSettings.Value.DefaultMarkerIcon;
        if (!string.IsNullOrEmpty(markerIcon))
        {
            options.Icon = new Icon { Url = markerIcon };
        }

        return options;
    }

    public async Task ClusterMarkersAsync(GoogleMap map1, IJSRuntime jsRuntime, bool fitMarkers = false)
    {
        MarkerCluster =
            await MarkerClustering.CreateAsync(jsRuntime, map1.InteropObject, markerList.Markers.Values.ToList());
    }

    public async Task<GoogleMapsComponents.Maps.MapOptions> GetLayoutAsync(bool miniView = false, double zoom = 9)
    {
        return new()
        {
            Zoom = (int)zoom,
            Center = new LatLngLiteral(4.52, 50.28),
            MapTypeId = MapTypeId.Roadmap,
            ZoomControl = !miniView,
            Scrollwheel = true,
            Draggable = true,
            MapTypeControl = !miniView,
            Styles = await GetMapStylesAsync()
        };
    }

    public async Task ShowMarkersAsync(GoogleMap map, IJSRuntime jsRuntime, List<MapMarker> markers,
        MapLayout mapLayout, Action<MouseEvent, string>? handleClick = null, Polypoint? zoomLocation = null)
    {
        var markersToShow = markers.Where(c => c.LocationEnabled);
        if (markersToShow.Count() != (markerList?.Markers?.Count ?? 0) || mapLayout.ShowMarkers == false)
        {
            await RemoveMarkersAsync();
        }

        if (!markersToShow?.Any() ?? false) return;

        await AddMarkersAsync(map, jsRuntime, markersToShow);
        if (zoomLocation != null)
        {
            await ZoomToLocationAsync(map, zoomLocation,  13);
        }
        else
        {
            await ZoomToMarkersAsync(map, markersToShow);
        }

        if (handleClick != null)
        {
            await markerList.AddListeners(markerList.Markers.Keys, "click", handleClick);
        }
    }

    private async Task ZoomToMarkersAsync(GoogleMap map, IEnumerable<MapMarker> markers)
    {
        var markersToShow = markers.Where(l => l.LocationEnabled);

        if (markersToShow.Count() > 1)
        {
            await map.InteropObject.FitBounds(new LatLngBoundsLiteral(
                new LatLngLiteral(
                    markersToShow.Min(l => l.Latitude),
                    markersToShow.Min(l => l.Longitude)),
                new LatLngLiteral(
                    markersToShow.Max(l => l.Latitude),
                    markersToShow.Max(l => l.Longitude))));
        }
        else
        {
            var center = markersToShow.First();
            await map.InteropObject.SetCenter(new LatLngLiteral(center.Latitude, center.Longitude));
            await map.InteropObject.SetZoom(15);
        }
    }

    private async Task ZoomToLocationAsync(GoogleMap map, Polypoint location, int zoom = 15)
    {
        await map.InteropObject.SetCenter(
            new LatLngLiteral(location.Latitude, location.Longitude));
        await map.InteropObject.SetZoom(zoom);
    }

    private async Task AddMarkersAsync(GoogleMap map, IJSRuntime jsRuntime, IEnumerable<MapMarker> markers)
    {
        if (markerList == null)
        {
            markerList = await MarkerList.CreateAsync(
                jsRuntime, new Dictionary<string, MarkerOptions>());
        }

        var markersToShow = markers.Where(l => l.LocationEnabled);
        await markerList.SetMultipleAsync(
            markersToShow
                .ToDictionary(s => s.Id, c => GetMarker(c, map)));
    }

    private async Task<MapTypeStyle[]> GetMapStylesAsync()
    {
        var result = mapOptions.GoogleMapStyle;
        if (string.IsNullOrEmpty(result))
        {
            var assembly = Assembly.GetAssembly(typeof(AeroMapService));
            var resourceName = "AeroBlazor.StaticData.defaultmapstyle.json";
            await using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    result = await reader.ReadToEndAsync();
                }
            }
        }

        var style = new GoogleMapStyleBuilder();
        style.AddStyle(result);
        return style.Build();
    }

    public async Task<InfoWindow> CreateInfoWindowAsync(MapMarker marker, IJSRuntime jsRuntime)
    {
        var infoWindowContent = new StringBuilder($@"<table class='table table-striped table-dark'><tbody>");
        infoWindowContent.Append($"<tr><td colspan=2><b>{marker.Label}</b></td></tr>");
        if (!string.IsNullOrEmpty(marker.Link))
        {
            infoWindowContent.Append(
                $"<tr><td colspan=2><a href='{marker.Link}' target='{getTarget(marker)}'>Details</a></td></tr>");
        }

        infoWindowContent.Append("<tr></tbody></table>");
        var infoWindow = await InfoWindow.CreateAsync(jsRuntime, new InfoWindowOptions()
        {
            Position = new LatLngLiteral(marker.Latitude,
                marker.Longitude)
        });
        await infoWindow.SetContent(infoWindowContent.ToString());
        return infoWindow;
    }

    private string getTarget(MapMarker marker)
    {
        return (marker.Link?.StartsWith("http") ?? false) ? "_blank" : "_self";
    }

    private async Task RemoveMarkersAsync()
    {
        if (markerList != null)
        {
            var tasks = new List<Task>();
            tasks.AddRange(markerList.Markers.Select(marker => marker.Value.SetMap(null)));
            await Task.WhenAll(tasks);
            await markerList.RemoveAllAsync();
        }
    }
}