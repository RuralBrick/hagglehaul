using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models
{
    /// <summary>
    /// The form to update a driver's information.
    /// </summary>
    [SwaggerSchema("Form to update a driver's information")]
    public class DriverUpdate
    {
        /// <summary>
        /// The new name of the driver.
        /// </summary>
        [SwaggerSchema("Name of the driver")]
        public string Name { get; set; }
        
        /// <summary>
        /// The new phone number of the driver.
        /// </summary>
        [SwaggerSchema("Phone number of the driver")]
        public string Phone { get; set; }
        
        /// <summary>
        /// The new make, model, year, and license plate of the driver's car.
        /// </summary>
        [SwaggerSchema("Description of the driver's car")]
        public string CarDescription { get; set; }
        
        /// <summary>
        /// The current password of the driver.
        /// </summary>
        [SwaggerSchema("Current password of the driver")]
        public string CurrentPassword { get; set; }
        
        /// <summary>
        /// The new password of the driver.
        /// </summary>
        [SwaggerSchema("New password of the driver")]
        public string NewPassword { get; set; }
    }
}
