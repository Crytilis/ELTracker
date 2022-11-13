using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;
using ELTracker.Logger;

namespace ELTracker.Services;

public class StatusService : DiscordClientService
{
    public StatusService(DiscordSocketClient client, ILogger<StatusService> logger) : base(client, logger)
    {
            
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Client.WaitForReadyAsync(stoppingToken);
        Log.WriteLine("EL-Tracker is ready and online!", LogStyle.Good);
        await Client.SetStatusAsync(UserStatus.Online);
        await Client.SetGameAsync("Monitoring Donations...", "", ActivityType.CustomStatus);
    }
}