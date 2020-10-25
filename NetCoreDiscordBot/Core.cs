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
                TotalShards = 1,
            };

            using (var services = ConfigureServices(config))
            {
                var _discordShardedClient = services.GetRequiredService<DiscordShardedClient>();
                string token = args[0];

                _discordShardedClient.ShardReady += ShardReadyAsync;
                _discordShardedClient.ShardReady += async (shard) =>
                {
                    await services.GetRequiredService<GuildDataExtensionsService>().InitializeAsync();
                    await services.GetRequiredService<GroupHandlingService>().InitializeAsync();
                    await services.GetRequiredService<GroupHandlingService>().LoadAllGroups();
                    await services.GetRequiredService<RoleDispenserService>().InitializeAsync();
                    await services.GetRequiredService<RoleDispenserService>().LoadAllDispensers();
                };
                _discordShardedClient.Log += LogAsync;

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                services.GetRequiredService<ReactionHandlingService>();


                await _discordShardedClient.LoginAsync(TokenType.Bot, token);
                await _discordShardedClient.StartAsync();
             
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
            serviceProvider.AddSingleton(new DiscordShardedClient(config));
            serviceProvider.AddSingleton<CommandService>();      
            serviceProvider.AddSingleton<CommandHandlingService>();
            serviceProvider.AddSingleton<ReactionHandlingService>();
            serviceProvider.AddSingleton<GuildDataExtensionsService>();
            serviceProvider.AddSingleton<GroupHandlingService>();
            serviceProvider.AddSingleton<RoleDispenserService>();
            serviceProvider.AddSingleton<BungieAPIService>();

            return serviceProvider.BuildServiceProvider();
        }
        private static Task ShardReadyAsync(DiscordSocketClient shard)
        {
            Console.WriteLine($"Shard Number {shard.ShardId} is connected and ready!");
            return Task.CompletedTask;
        }

        private static Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }
    }
}
