namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Form to log in to the application.
    /// </summary>
    public class Login
    {
        /// <summary>
        /// Email address of the user. User type is inferred from this.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Password of the user.
        /// </summary>
        public string Password { get; set; }
    }
}
