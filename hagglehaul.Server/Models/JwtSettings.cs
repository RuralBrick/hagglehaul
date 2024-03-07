namespace hagglehaul.Server.Models
{
    /// <summary>
    /// JWT settings for the application, used for client authentication.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Valid audience for the JWT.
        /// </summary>
        public string ValidAudience { get; set; } = null!;
        
        /// <summary>
        /// Valid issuer for the JWT.
        /// </summary>
        public string ValidIssuer { get; set; } = null!;
        
        /// <summary>
        /// Secret key for the JWT, encoded as a string.
        /// </summary>
        public string Secret { get; set; } = null!;
    }
}
