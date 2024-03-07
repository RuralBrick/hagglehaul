using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models
{
    [SwaggerSchema("Form to update a driver's information")]
    public class DriverUpdate
    {
        [SwaggerSchema("Name of the driver")]
        public string Name { get; set; }

        [SwaggerSchema("Phone number of the driver")]
        public string Phone { get; set; }

        [SwaggerSchema("Description of the driver's car")]
        public string CarDescription { get; set; }

        [SwaggerSchema("Current password of the driver")]
        public string CurrentPassword { get; set; }

        [SwaggerSchema("New password of the driver")]
        public string NewPassword { get; set; }
    }
}
