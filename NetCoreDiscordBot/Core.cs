using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using NetCoreDiscordBot.Models;
using NetCoreDiscordBot.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreDiscordBot.Models.Groups;
using NetCoreDiscordBot.Models.Guilds;
using NetCoreDiscordBot.Models.Dispensers;

namespace NetCoreDiscordBot
{
    public class Core
    {
        public static IConfigurationRoot Configuration { get; private set; }
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                throw new ArgumentNullException("Missing token for bot");
            }
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

            using (var services = ConfigureServices(config))
            {
                var _discordsocketClient = services.GetRequiredService<DiscordSocketClient>();
                string token = args[0];

                _discordsocketClient.Ready += ReadyAsync;               
                _discordsocketClient.Log += LogAsync;

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                services.GetRequiredService<ReactionHandlingService>();

                _discordsocketClient.Ready += async () =>
                {
                    foreach (var guild in _discordsocketClient.Guilds)
                    {
                        await guild.DownloadUsersAsync();
                        Console.WriteLine($"{guild.Users.Count} users loaded for {guild.Name}");
                    }
                    await services.GetRequiredService<GuildDataExtensionsService>().InitializeAsync();
                    await services.GetRequiredService<GroupHandlingService>().InitializeAsync();
                    await services.GetRequiredService<GroupHandlingService>().LoadAllGroups();
                    await services.GetRequiredService<RoleDispenserService>().InitializeAsync();
                    await services.GetRequiredService<RoleDispenserService>().LoadAllDispensers();
                    Console.WriteLine("Bot ready.");
                };
                
                await _discordsocketClient.LoginAsync(TokenType.Bot, token);
                await _discordsocketClient.StartAsync();
                
                await Task.Delay(Timeout.Infinite);
            }          
        }

        private static ServiceProvider ConfigureServices(DiscordSocketConfig config)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configBuilder.AddJsonFile("appsettings.json");
            Configuration = configBuilder.Build();

            var serviceProvider = new ServiceCollection();
            serviceProvider.AddSingleton<DebugService>();
            serviceProvider.AddDbContext<GroupsContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionGroups")));
            serviceProvider.AddDbContext<GuildDataExtensionsContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionExtensions")));
            serviceProvider.AddDbContext<RoleDispensersContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionDispensers")));
            serviceProvider.AddSingleton(new DiscordSocketClient(config));
            serviceProvider.AddSingleton<CommandService>();      
            serviceProvider.AddSingleton<CommandHandlingService>();
            serviceProvider.AddSingleton<ReactionHandlingService>();
            serviceProvider.AddSingleton<GuildDataExtensionsService>();
            serviceProvider.AddSingleton<GroupHandlingService>();
            serviceProvider.AddSingleton<RoleDispenserService>();
            serviceProvider.AddSingleton<BungieAPIService>();

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
    }
}
