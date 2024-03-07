namespace hagglehaul.Server.Models;

/// <summary>
/// Settings for the Mapbox API, to be used by backend services.
/// </summary>
public class MapboxSettings
{
    /// <summary>
    /// Access token for the Mapbox API. Must have access to navigation and geocoding APIs.
    /// </summary>
    public string AccessToken { get; set; } = null!;
}
