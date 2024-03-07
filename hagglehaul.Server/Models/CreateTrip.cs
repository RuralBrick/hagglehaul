using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models
{
    [SwaggerSchema("Form to create a new trip")]
    public class CreateTrip
    {
        [SwaggerSchema("Name of the trip")]
        public string Name { get; set; }

        [SwaggerSchema("Start time of the trip")]
        public DateTime StartTime { get; set; }

        [SwaggerSchema("Start longitude of the trip")]
        public double PickupLong { get; set; }

        [SwaggerSchema("Start latitude of the trip")]
        public double PickupLat { get; set; }

        [SwaggerSchema("End longitude of the trip")]
        public double DestinationLong { get; set; }

        [SwaggerSchema("End latitude of the trip")]
        public double DestinationLat { get; set; }

        [SwaggerSchema("Address of the pickup location")]
        public string PickupAddress { get; set; }

        [SwaggerSchema("Address of the destination")]
        public string DestinationAddress { get; set; }

        [SwaggerSchema("Size of the rider's party")]
        public uint PartySize { get; set; }
    }
}
