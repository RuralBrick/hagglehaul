namespace hagglehaul.Server.Models
{
    /// <summary>
    /// The form to create a trip.
    /// </summary>
    public class CreateTrip
    {
        /// <summary>
        /// The name of the trip.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The start time of the trip.
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// The longitude of the pickup location.
        /// </summary>
        public double PickupLong { get; set; }
        
        /// <summary>
        /// The latitude of the pickup location.
        /// </summary>
        public double PickupLat { get; set; }
        
        /// <summary>
        /// The longitude of the destination location.
        /// </summary>
        public double DestinationLong { get; set; }
        
        /// <summary>
        /// The latitude of the destination location.
        /// </summary>
        public double DestinationLat { get; set; }
        
        /// <summary>
        /// The address of the pickup location.
        /// </summary>
        public string PickupAddress { get; set; }
        
        /// <summary>
        /// The address of the destination.
        /// </summary>
        public string DestinationAddress { get; set; }
        
        /// <summary>
        /// The number of riders in the trip.
        /// </summary>
        public uint PartySize { get; set; }
    }
}
