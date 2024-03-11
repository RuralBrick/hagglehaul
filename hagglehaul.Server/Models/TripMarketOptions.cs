using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Options to search for trips in the market.
    /// </summary>
    [SwaggerSchema("Options for filtering and sorting trips in the market")]
    public class TripMarketOptions
    {
        /// <summary>
        /// Current latitude of the driver, WGS84.
        /// </summary>
        [SwaggerSchema("Current latitude of the driver")]
        public double? CurrentLat { get; set; }

        /// <summary>
        /// Current longitude of the driver, WGS84.
        /// </summary>
        [SwaggerSchema("Current longitude of the driver")]
        public double? CurrentLong { get; set; }

        /// <summary>
        /// Latitude driver wants to go to after the trip, WGS84.
        /// </summary>
        [SwaggerSchema("Latitude driver wants to go to after the trip")]
        public double? TargetLat { get; set; }

        /// <summary>
        /// Longitude driver wants to go to after the trip, WGS84.
        /// </summary>
        [SwaggerSchema("Longitude driver wants to go to after the trip")]
        public double? TargetLong { get; set; }

        /// <summary>
        /// Maximum straight-line distance from current coordinates to start coordinates to filter by, in WGS84 coordinate distance.
        /// </summary>
        [SwaggerSchema("Maximum straight-line distance from current coordinates to start coordinates to filter by")]
        public double? MaxCurrentToStartDistance { get; set; }

        /// <summary>
        /// Maximum straight-line distance from end coordinates to target coordinates to filter by, in WGS84 coordinate distance.
        /// </summary>
        [SwaggerSchema("Maximum straight-line distance from end coordinates to target coordinates to filter by")]
        public double? MaxEndToTargetDistance { get; set; }

        /// <summary>
        /// Maximum straight-line distance from start coordinates to end coordinates to filter by, in WGS84 coordinate distance.
        /// </summary>
        [SwaggerSchema("Maximum straight-line distance from start coordinates to end coordinates to filter by")]
        public double? MaxEuclideanDistance { get; set; }

        /// <summary>
        /// Maximum route distance to filter by, in meters.
        /// </summary>
        [SwaggerSchema("Maximum route distance to filter by")]
        public double? MaxRouteDistance { get; set; }

        /// <summary>
        /// Minimum amount that the current minimum bid is at to filter by, in cents.
        /// </summary>
        [SwaggerSchema("Minimum amount that the current minimum bid is at to filter by")]
        public double? MinCurrentMinBid { get; set; }

        /// <summary>
        /// List of methods to sort by from highest to lowest priority; Can be
        /// "euclideanDistance", "routeDistance", "routeDuration", "currentToStartDistance",
        /// "endToTargetDistance", "currentMinBid", and "startTime".
        /// </summary>
        [SwaggerSchema("List of methods to sort by from highest to lowest priority; Can be \"euclideanDistance\", \"routeDistance\", \"routeDuration\", \"currentToStartDistance\", \"endToTargetDistance\", \"currentMinBid\", and \"startTime\"")]
        public List<string> SortMethods { get; set; } = new List<string>();
    }
}
