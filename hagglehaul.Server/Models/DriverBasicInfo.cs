namespace hagglehaul.Server.Models;

/// <summary>
/// Data for displaying a driver's basic information.
/// </summary>
public class DriverBasicInfo
{
    /// <summary>
    /// The name of the driver.
    /// </summary>
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// The email address of the driver.
    /// </summary>
    public string Email { get; set; } = null!;
    
    /// <summary>
    /// The phone number of the driver.
    /// </summary>
    public string Phone { get; set; } = null!;
    
    /// <summary>
    /// The make, model, year, and license plate of the driver's car.
    /// </summary>
    public string CarDescription { get; set; } = null!;
}
