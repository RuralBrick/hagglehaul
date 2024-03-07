using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Controllers;

/// <summary>
/// Controller for geographic place lookup.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PlaceLookupController : ControllerBase
{
    private readonly IGeographicRouteService _geographicRouteService;
    
    public PlaceLookupController(IGeographicRouteService geographicRouteService)
    {
        _geographicRouteService = geographicRouteService;
    }
    
    /// <summary>
    /// Lookup geographic place by name.
    /// </summary>
    /// <param name="placeName">The search string (as put in a search bar)</param>
    /// <returns>
    /// <see cref="ContentResult"/> with the geographic place found,
    /// <see cref="BadRequestObjectResult"/> if the request has an empty place name,
    /// <see cref="StatusCodeResult"/> with status code 500 if the geographic place lookup failed
    /// </returns>
    [Authorize]
    [HttpGet(Name = "GetPlaceLookup")]
    [SwaggerOperation(Summary = "Lookup geographic place by name")]
    [SwaggerResponse(StatusCodes.Status200OK, "Geographic place found")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request has empty place name")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Geographic place lookup failed")]
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
