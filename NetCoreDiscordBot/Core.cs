using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NetCoreDiscordBot.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using NetCoreDiscordBot.CustomServices.DecoderPuzzle.Services;
using NetCoreDiscordBot.Services.Interfaces;

namespace NetCoreDiscordBot
{
    public class Core
    {
        private static IServiceProvider _services;
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                throw new ArgumentNullException("Missing token for bot");
            }
            AppDomain.CurrentDomain.ProcessExit += OnAppClosing;

            MainAsync(args).GetAwaiter().GetResult();

        }

        private static async Task MainAsync(string[] args)
        {
            var config = new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 10,
                AlwaysDownloadUsers = true
            };

            var services = ConfigureServices(config);
            _services = services;
            var _discordsocketClient = services.GetRequiredService<DiscordSocketClient>();
            string token = args[0];

            _discordsocketClient.Ready += ReadyAsync;
            _discordsocketClient.Log += LogAsync;

            services.GetRequiredService<IBungieService>().Client.LogListener.OnNewMessage += LogFromBungieAsync;
            services.GetRequiredService<IBungieService>().Client.Run().GetAwaiter().GetResult();

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
            services.GetRequiredService<ReactionHandlingService>();

            _discordsocketClient.Ready += async () =>
            {
                foreach (var guild in _discordsocketClient.Guilds)
                {
                    await guild.DownloadUsersAsync();
                    Console.WriteLine($"{guild.Users.Count} users loaded for {guild.Name}");
                }
                await services.GetRequiredService<IGroupHandlerService>().InitializeAsync();
                await services.GetRequiredService<IRoleDispenserService>().InitializeAsync();
                await services.GetRequiredService<IGuildDataExtensionsService>().InitializeAsync();

                services.GetRequiredService<LockPuzzleService>().Init();
                //services.GetRequiredService<IMongoDBAccessService>();

                Console.WriteLine("Bot ready.");
            };

            await _discordsocketClient.LoginAsync(TokenType.Bot, token);
            await _discordsocketClient.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }
        private static ServiceProvider ConfigureServices(DiscordSocketConfig config)
        {
            var serviceProvider = new ServiceCollection();

            serviceProvider.AddSingleton<IConfigurationService, ConfigurationService>();
            serviceProvider.AddSingleton<ILoggerService, LoggerService>();
            serviceProvider.AddSingleton<IMongoDBAccessService, MongoDBAccessService>();
            serviceProvider.AddSingleton<IGroupHandlerService, GroupHandlerService>();
            serviceProvider.AddSingleton<IRoleDispenserService, RoleDispenserService>();
            serviceProvider.AddSingleton<IGuildDataExtensionsService, GuildDataExtensionsService>();
            serviceProvider.AddSingleton<IBungieService, BungieService>();


            serviceProvider.AddSingleton(new DiscordSocketClient(config));
            serviceProvider.AddSingleton<CommandService>();      
            serviceProvider.AddSingleton<CommandHandlingService>();
            serviceProvider.AddSingleton<ReactionHandlingService>();
            serviceProvider.AddSingleton<LockPuzzleService>();

            return serviceProvider.BuildServiceProvider();
        }
        private static Task ReadyAsync()
        {
            Console.WriteLine($"Ready!");
            return Task.CompletedTask;
        }
        private static Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }
        private static void LogFromBungieAsync(BungieNetCoreAPI.Logging.LogMessage logMessage)
        {
            Console.WriteLine(logMessage.ToString());
        }
        private static void OnAppClosing(object sender, EventArgs e)
        {
            _services.GetService<IMongoDBAccessService>().KillMongoDBProcess();
        }
    }
}
