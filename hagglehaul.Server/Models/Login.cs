using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Form to log in to the application.
    /// </summary>
    [SwaggerSchema("Form to login to the application")]
    public class Login
    {
        /// <summary>
        /// Email address of the user. User type is inferred from this.
        /// </summary>
        [SwaggerSchema("Email of the user")]
        public string Email { get; set; }
        
        /// <summary>
        /// Password of the user.
        /// </summary>
        [SwaggerSchema("Password of the user")]
        public string Password { get; set; }
    }
}
