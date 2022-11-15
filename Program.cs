using Discord;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;
using ELTracker.Managers;
using ELTracker.Services;
using ELTracker.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using Serilog;

namespace ELTracker;

public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Level:w4}] {Message:lj}{NewLine}{Exception}", theme: SerilogThemes.ExtraLifeTracker)
            .CreateLogger();

        try
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "The host terminated unexpectedly\r\n\t{message}\r\n\t{stack}", ex.Message, ex.StackTrace);
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
            .UseSerilog()
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
                    var serializerSettings = new JsonSerializerSettings {
                        ContractResolver     = new CamelCasePropertyNamesContractResolver(),
                        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                        TypeNameHandling     = TypeNameHandling.Auto,
                        NullValueHandling    = NullValueHandling.Ignore,
                        Formatting           = Formatting.None,
                        ConstructorHandling  = ConstructorHandling.AllowNonPublicDefaultConstructor
                    };
                    var restSettings = sp.GetRequiredService<IOptions<RestSettings>>().Value;
                    var restClient = new RestClient(new RestClientOptions(restSettings.BaseUrl));
                    restClient.UseNewtonsoftJson(serializerSettings);
                    return restClient;
                });
                services.AddScoped(typeof(IServerManager), typeof(ServerManager));
                services.AddScoped(typeof(IDonationManager), typeof(DonationManager));
                services.AddHostedService<InteractionHandler>();
                services.AddHostedService<StatusService>();
                services.AddHostedService<TrackingService>();
            });
    }
}