namespace hagglehaul.Server.Models
{
    public class JwtSettings
    {
        public string ValidAudience { get; set; } = null!;
        public string ValidIssuer { get; set; } = null!;
        public string Secret { get; set; } = null!;
    }
}
