namespace hagglehaul.Server.Models;

/// <summary>
/// Form to register a new user.
/// </summary>
public class Register
{
    /// <summary>
    /// The email address of the user.
    /// </summary>
    public string Email { get; set; }
    
    /// <summary>
    /// The password of the user.
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    /// The role of the user. Must be either "rider" or "driver".
    /// </summary>
    public string Role { get; set; }
}
