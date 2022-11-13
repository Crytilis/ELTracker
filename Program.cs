using Discord;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;
using ELTracker.Logger;
using ELTracker.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RestSharp;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ELTracker;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }
        catch (Exception e)
        {
            Log.WriteException(e);
        }
            
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config
                    .SetBasePath(context.HostingEnvironment.ContentRootPath)
                    .AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: true)
                    .Build();
            })
            .ConfigureLogging(x =>
            {
                x.AddConsole();
                x.SetMinimumLevel(LogLevel.Debug);
            })
            .ConfigureDiscordHost((context, config) =>
            {
                config.SocketConfig = new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers |
                                     GatewayIntents.GuildMessageReactions | GatewayIntents.GuildMessages |
                                     GatewayIntents.MessageContent,
                    LogLevel = LogSeverity.Debug,
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 200
                };
                config.Token = context.Configuration["BotSettings:Token"]!;
            })
            .UseInteractionService((_, config) =>
            {
                config.LogLevel = LogSeverity.Info;
                config.UseCompiledLambda = false;
                config.AutoServiceScopes = true;
                config.DefaultRunMode = RunMode.Async;
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<HostOptions>(option => { option.ShutdownTimeout = TimeSpan.FromSeconds(10); });
                services.Configure<DatabaseSettings>(context.Configuration.GetSection(nameof(DatabaseSettings)));
                services.AddSingleton(sp =>
                {
                    var databaseSettings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                    var mongoSettings = MongoClientSettings.FromConnectionString(databaseSettings.ConnectionString);
                    mongoSettings.MinConnectionPoolSize = 100;
                    mongoSettings.MaxConnectionPoolSize = 500;
                    mongoSettings.WaitQueueTimeout = TimeSpan.FromSeconds(15);
                    var databaseClient = new MongoClient(mongoSettings);
                    var database = databaseClient.GetDatabase(databaseSettings.DatabaseName);
                    return database;
                });
                services.Configure<RestSettings>(context.Configuration.GetSection(nameof(RestSettings)));
                services.AddSingleton(sp =>
                {
                    var restSettings = sp.GetRequiredService<IOptions<RestSettings>>().Value;
                    var restClient = new RestClient(new RestClientOptions(restSettings.BaseUrl));
                    return restClient;
                });
                // services.AddScoped(typeof(IGuildManager<>), typeof(GuildManager<>));
                // services.AddScoped(typeof(IDonorManager<>), typeof(DonorManager<>));
                // services.AddScoped(typeof(IAltDonorManager<>), typeof(AltDonorManager<>));
                // services.AddScoped(typeof(ITotalManager), typeof(TotalManager));
                // services.AddScoped<IGuildService, GuildService>();
                // services.AddScoped<IDonorService, DonorService>();
                // services.AddHostedService<InteractionHandler>();
                // services.AddHostedService<StatusService>();
                // services.AddHostedService<DonationTracker>();
                // services.AddHostedService<RoleService>();
            });
    }
}