namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Form to update a rider's information.
    /// </summary>
    public class RiderUpdate
    {
        /// <summary>
        /// The new name of the rider.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The new phone number of the rider.
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// The current password of the rider.
        /// </summary>
        public string CurrentPassword { get; set; }
        
        /// <summary>
        /// The new password of the rider.
        /// </summary>
        public string NewPassword { get; set; }
    }
}
