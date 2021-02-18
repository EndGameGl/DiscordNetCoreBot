using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NetCoreDiscordBot.Services.Interfaces;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services
{
    public class CommandHandlingService
    {
        public readonly CommandService Commands;
        private readonly DiscordSocketClient _discordClient;
        private readonly IServiceProvider _services;
        private readonly IGuildDataExtensionsService _dataExtensionsService;
        private const char _defaultCommandPrefix = '_';
        public CommandHandlingService(IServiceProvider services)
        {
            Commands = services.GetRequiredService<CommandService>();
            _discordClient = services.GetRequiredService<DiscordSocketClient>();
            _dataExtensionsService = services.GetRequiredService<IGuildDataExtensionsService>();
            _services = services;

            Commands.CommandExecuted += CommandExecutedAsync;
            Commands.Log += LogAsync;
            _discordClient.MessageReceived += MessageReceivedAsync;
        }
        public async Task InitializeAsync()
        {
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
        private async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message))
                return;
            if (message.Source != MessageSource.User)
                return;
            var argPos = 0;
            if (!message.HasCharPrefix('_', ref argPos))
                return;
            if (rawMessage.Channel is SocketGuildChannel channel)
            {
                if (_dataExtensionsService.TryGetData(channel.Guild.Id, out var data))
                {
                    if (!message.HasCharPrefix(data.CommandPrefix, ref argPos))
                        return;
                }
            }
            var context = new SocketCommandContext(_discordClient, message);
            await Commands.ExecuteAsync(context, argPos, _services);
        }
        private async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified)
                return;
            if (result.IsSuccess)
                return;
            await context.Channel.SendMessageAsync($"error: {result.ToString()}");
        }
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }
    }
}
