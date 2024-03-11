namespace hagglehaul.Server.Models;

/// <summary>
/// Settings for the Hagglehaul database.
/// </summary>
public class HagglehaulDatabaseSettings
{
    /// <summary>
    /// The connection string to the Hagglehaul database.
    /// </summary>
    public string ConnectionString { get; set; } = null!;
    
    /// <summary>
    /// The name of the Hagglehaul database. Must match the name of the database within MongoDB.
    /// </summary>
    public string DatabaseName { get; set; } = null!;   
}
