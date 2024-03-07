namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Options to search for trips in the market.
    /// </summary>
    public class TripMarketOptions
    {
        public double? CurrentLat { get; set; }
        public double? CurrentLong { get; set; }
        public double? TargetLat { get; set; }
        public double? TargetLong { get; set; }
        public double? MaxCurrentToStartDistance { get; set; }
        public double? MaxEndToTargetDistance { get; set; }
        public double? MaxEuclideanDistance { get; set; }
        public double? MaxRouteDistance { get; set; }
        public double? MinCurrentMinBid { get; set; }
        public List<string> SortMethods { get; set; } = new List<string>();
    }
}
