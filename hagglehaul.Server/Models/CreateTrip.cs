namespace hagglehaul.Server.Models
{
    public class CreateTrip
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public double PickupLong { get; set; }
        public double PickupLat { get; set; }
        public double DestinationLong { get; set; }
        public double DestinationLat { get; set; }
        public string PickupAddress { get; set; }
        public string DestinationAddress { get; set; }
        public uint PartySize { get; set; }
    }
}
