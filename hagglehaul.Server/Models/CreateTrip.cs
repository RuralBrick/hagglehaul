namespace hagglehaul.Server.Models
{
    public class CreateTrip
    {
        public DateTime StartTime { get; set; }
        public double PickupLong { get; set; }
        public double PickupLat { get; set; }
        public double DestinationLong { get; set; }
        public double DestinationLat { get; set; }
    }
}
