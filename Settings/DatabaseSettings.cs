namespace ELTracker.Settings;

internal interface IDatabaseOptions
{
    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
}

internal class DatabaseSettings : IDatabaseOptions
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}