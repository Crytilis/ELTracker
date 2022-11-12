namespace ELTracker.Settings;

internal interface IBotSettings
{
    string Token { get; set; }
    ulong DevGuild { get; set; }
}

internal class BotSettings : IBotSettings
{
    public string Token { get; set; }
    public ulong DevGuild { get; set; }
}