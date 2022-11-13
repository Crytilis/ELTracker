namespace ELTracker.Settings;

public interface IDatabaseOptions
{
    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
}

public class DatabaseSettings : IDatabaseOptions
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}