using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models;

[SwaggerSchema("Form to register a new user")]
public class Register
{
    [SwaggerSchema("Email of the user")]
    public string Email { get; set; }

    [SwaggerSchema("Password of the user")]
    public string Password { get; set; }

    [SwaggerSchema("Role of the user; either \"rider\" or \"driver\"")]
    public string Role { get; set; }
}
