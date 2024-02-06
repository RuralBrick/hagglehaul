using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hagglehaul.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaceLookupController : ControllerBase
{
    private readonly IGeographicRouteService _geographicRouteService;
    
    public PlaceLookupController(IGeographicRouteService geographicRouteService)
    {
        _geographicRouteService = geographicRouteService;
    }
    
    [Authorize]
    [HttpGet(Name = "GetPlaceLookup")]
    public async Task<IActionResult> Get([FromQuery] string placeName)
    {
        if (String.IsNullOrEmpty(placeName))
            return BadRequest("Request has empty place name");
        
        var features = await _geographicRouteService.GeocodingLookup(placeName);
        if (String.IsNullOrEmpty(features))
            return StatusCode(500);

        return this.Content(features, "application/json");
    }
}
