using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models
{
    /// <summary>
    /// The form to create a trip.
    /// </summary>
    [SwaggerSchema("Form to create a new trip")]
    public class CreateTrip
    {
        /// <summary>
        /// The name of the trip.
        /// </summary>
        [SwaggerSchema("Name of the trip")]
        public string Name { get; set; }
        
        /// <summary>
        /// The start time of the trip.
        /// </summary>
        [SwaggerSchema("Start time of the trip")]
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// The longitude of the pickup location, WGS84.
        /// </summary>
        [SwaggerSchema("Start longitude of the trip")]
        public double PickupLong { get; set; }
        
        /// <summary>
        /// The latitude of the pickup location, WGS84.
        /// </summary>
        [SwaggerSchema("Start latitude of the trip")]
        public double PickupLat { get; set; }
        
        /// <summary>
        /// The longitude of the destination location, WGS84.
        /// </summary>
        [SwaggerSchema("End longitude of the trip")]
        public double DestinationLong { get; set; }
        
        /// <summary>
        /// The latitude of the destination location, WGS84.
        /// </summary>
        [SwaggerSchema("End latitude of the trip")]
        public double DestinationLat { get; set; }
        
        /// <summary>
        /// The address of the pickup location.
        /// </summary>
        [SwaggerSchema("Address of the pickup location")]
        public string PickupAddress { get; set; }
        
        /// <summary>
        /// The address of the destination.
        /// </summary>
        [SwaggerSchema("Address of the destination")]
        public string DestinationAddress { get; set; }
        
        /// <summary>
        /// The number of riders in the trip.
        /// </summary>
        [SwaggerSchema("Size of the rider's party")]
        public uint PartySize { get; set; }
    }
}
