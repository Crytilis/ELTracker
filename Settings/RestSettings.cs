namespace ELTracker.Settings;

public interface IRestSettings
{
    string BaseUrl { get; set; }
}

public class RestSettings : IRestSettings
{
    public string BaseUrl { get; set; }
}