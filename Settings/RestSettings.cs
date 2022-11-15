namespace ELTracker.Settings;

public interface IRestSettings
{
    string BaseUrl { get; set; }
    string ParticipantId { get; set; }
    string RequestUri { get; }
}

public class RestSettings : IRestSettings
{
    public string BaseUrl { get; set; }
    public string ParticipantId { get; set; }

    public string RequestUri
    {
        get
        {
            var uri = $"{BaseUrl}/{ParticipantId}/donors";
            return uri;
        }
    }
}