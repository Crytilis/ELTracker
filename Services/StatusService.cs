using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;
using Serilog;
using Serilog.Events;

namespace ELTracker.Services;

public class StatusService : DiscordClientService
{
    public StatusService(DiscordSocketClient client, ILogger<StatusService> logger) : base(client, logger)
    {
            
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Client.WaitForReadyAsync(stoppingToken);
        Log.Write(LogEventLevel.Warning, "{username} is now online.", Client.CurrentUser.Username);
        await Client.SetStatusAsync(UserStatus.Online);
        await Client.SetGameAsync("Monitoring Donations...", "", ActivityType.CustomStatus);
    }
}