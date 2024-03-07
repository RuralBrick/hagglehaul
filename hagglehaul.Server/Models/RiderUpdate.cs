using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models
{
    [SwaggerSchema("Form to update a rider's information")]
    public class RiderUpdate
    {
        [SwaggerSchema("Name of the rider")]
        public string Name { get; set; }

        [SwaggerSchema("Phone number of the rider")]
        public string Phone { get; set; }

        [SwaggerSchema("Current password of the rider")]
        public string CurrentPassword { get; set; }

        [SwaggerSchema("New password of the rider")]
        public string NewPassword { get; set; }
    }
}
