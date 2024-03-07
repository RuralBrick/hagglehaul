using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models
{
    [SwaggerSchema("Form to login to the application")]
    public class Login
    {
        [SwaggerSchema("Email of the user")]
        public string Email { get; set; }

        [SwaggerSchema("Password of the user")]
        public string Password { get; set; }
    }
}
