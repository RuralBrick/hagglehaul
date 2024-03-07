using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models;

/// <summary>
/// Form to register a new user.
/// </summary>
[SwaggerSchema("Form to register a new user")]
public class Register
{
    /// <summary>
    /// The email address of the user.
    /// </summary>
    [SwaggerSchema("Email of the user")]
    public string Email { get; set; }
    
    /// <summary>
    /// The password of the user.
    /// </summary>
    [SwaggerSchema("Password of the user")]
    public string Password { get; set; }
    
    /// <summary>
    /// The role of the user. Must be either "rider" or "driver".
    /// </summary>
    [SwaggerSchema("Role of the user; either \"rider\" or \"driver\"")]
    public string Role { get; set; }
}
