using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models
{
    [SwaggerSchema("Options for filtering and sorting trips in the market")]
    public class TripMarketOptions
    {
        [SwaggerSchema("Current latitude of the driver")]
        public double? CurrentLat { get; set; }

        [SwaggerSchema("Current longitude of the driver")]
        public double? CurrentLong { get; set; }

        [SwaggerSchema("Latitude driver wants to go to after the trip")]
        public double? TargetLat { get; set; }

        [SwaggerSchema("Longitude driver wants to go to after the trip")]
        public double? TargetLong { get; set; }

        [SwaggerSchema("Maximum straight-line distance from current coordinates to start coordinates to filter by")]
        public double? MaxCurrentToStartDistance { get; set; }

        [SwaggerSchema("Maximum straight-line distance from end coordinates to target coordinates to filter by")]
        public double? MaxEndToTargetDistance { get; set; }

        [SwaggerSchema("Maximum straight-line distance from start coordinates to end coordinates to filter by")]
        public double? MaxEuclideanDistance { get; set; }

        [SwaggerSchema("Maximum route distance to filter by")]
        public double? MaxRouteDistance { get; set; }

        [SwaggerSchema("Minimum amount that the current minimum bid is at to filter by")]
        public double? MinCurrentMinBid { get; set; }

        [SwaggerSchema("List of methods to sort by from highest to lowest priority; Can be \"euclideanDistance\", \"routeDistance\", \"routeDuration\", \"currentToStartDistance\", \"endToTargetDistance\", \"currentMinBid\", and \"startTime\"")]
        public List<string> SortMethods { get; set; } = new List<string>();
    }
}
