namespace hagglehaul.Server.Models
{
    /// <summary>
    /// The form to update a driver's information.
    /// </summary>
    public class DriverUpdate
    {
        /// <summary>
        /// The new name of the driver.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The new phone number of the driver.
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// The new make, model, year, and license plate of the driver's car.
        /// </summary>
        public string CarDescription { get; set; }
        
        /// <summary>
        /// The current password of the driver.
        /// </summary>
        public string CurrentPassword { get; set; }
        
        /// <summary>
        /// The new password of the driver.
        /// </summary>
        public string NewPassword { get; set; }
    }
}
