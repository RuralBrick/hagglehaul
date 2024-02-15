namespace hagglehaul.Server.Models;

public class EmailSettings
{
    public string ConnectionString { get; set; } = null!;
    public string SenderAddress { get; set; } = null!;
}
