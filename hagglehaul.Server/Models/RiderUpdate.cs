using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Form to update a rider's information.
    /// </summary>
    [SwaggerSchema("Form to update a rider's information")]
    public class RiderUpdate
    {
        /// <summary>
        /// The new name of the rider.
        /// </summary>
        [SwaggerSchema("Name of the rider")]
        public string Name { get; set; }
        
        /// <summary>
        /// The new phone number of the rider.
        /// </summary>
        [SwaggerSchema("Phone number of the rider")]
        public string Phone { get; set; }
        
        /// <summary>
        /// The current password of the rider.
        /// </summary>
        [SwaggerSchema("Current password of the rider")]
        public string CurrentPassword { get; set; }
        
        /// <summary>
        /// The new password of the rider.
        /// </summary>
        [SwaggerSchema("New password of the rider")]
        public string NewPassword { get; set; }
    }
}
