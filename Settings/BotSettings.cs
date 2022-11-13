namespace ELTracker.Settings;

public interface IBotSettings
{
    string Token { get; set; }
    ulong DevGuild { get; set; }
}

public class BotSettings : IBotSettings
{
    public string Token { get; set; }
    public ulong DevGuild { get; set; }
}