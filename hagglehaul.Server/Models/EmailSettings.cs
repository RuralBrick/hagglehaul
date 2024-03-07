namespace hagglehaul.Server.Models;

/// <summary>
/// Settings for Azure Communication Services.
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// The connection string for Azure Communication Services.
    /// </summary>
    public string ConnectionString { get; set; } = null!;
    
    /// <summary>
    /// The sender address for emails within the ACS system.
    /// </summary>
    public string SenderAddress { get; set; } = null!;
}
