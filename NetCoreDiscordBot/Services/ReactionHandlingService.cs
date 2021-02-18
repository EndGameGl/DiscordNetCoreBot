using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NetCoreDiscordBot.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services
{
    public class ReactionHandlingService
    {
        private readonly IGuildDataExtensionsService _dataExtensionsService;
        private readonly DiscordSocketClient _discordClient;
        private readonly IConfigurationService _config;
        private readonly IGroupHandlerService _groupHandlingService;
        private readonly IRoleDispenserService _roleDispenserService;
        public ReactionHandlingService(IServiceProvider services)
        {
            _discordClient = services.GetRequiredService<DiscordSocketClient>();
            _groupHandlingService = services.GetRequiredService<IGroupHandlerService>();
            _roleDispenserService = services.GetRequiredService<IRoleDispenserService>();
            _dataExtensionsService = services.GetRequiredService<IGuildDataExtensionsService>();
            _config = services.GetRequiredService<IConfigurationService>();

            _discordClient.ReactionAdded += ReactionAddedHandler;
            _discordClient.ReactionRemoved += ReactionRemovedHandler;
        }

        private async Task ReactionAddedHandler(Cacheable<IUserMessage, ulong> messageCache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.IsSpecified && !reaction.User.Value.IsBot)
            {
                if (channel is SocketGuildChannel guildChannel)
                {
                    if (_groupHandlingService.TryGetGroup(guildChannel.Guild.Id, reaction.MessageId, out var connectedGroup))
                    {
                        if (reaction.Emote.Name == _config.Configuration["Emojis:DefaultCloseEmoji"])
                        {
                            if (reaction.UserId == connectedGroup.Host.Id || _dataExtensionsService.IsAdmin(connectedGroup.Guild.Id, reaction.UserId))
                            {
                                await connectedGroup.CloseMessage();
                                await _groupHandlingService.RemoveGroup(connectedGroup);
                            }
                        }
                        else if (reaction.Emote.Name == _config.Configuration["Emojis:DefaultCallEmoji"])
                        {
                            if (reaction.UserId == connectedGroup.Host.Id || _dataExtensionsService.IsAdmin(connectedGroup.Guild.Id, reaction.UserId))
                            {
                                await connectedGroup.SendAnnouncement(guildChannel.Guild.GetUser(reaction.UserId));
                            }
                        }
                        else
                        {
                            var userList = connectedGroup.UserLists.FirstOrDefault(x => x.JoinEmote.Name == reaction.Emote.Name);
                            if (userList != null)
                            {
                                var user = guildChannel.GetUser(reaction.UserId);
                                if (userList.TryJoinList(user))
                                {
                                    await _groupHandlingService.ReplaceWithNewerGroup(connectedGroup);
                                }
                                else
                                    await connectedGroup.PresentationMessage.RemoveReactionAsync(reaction.Emote, user);
                            }
                        }
                    }
                    else if (_roleDispenserService.TryGetRoleDispenser(guildChannel.Guild.Id, reaction.MessageId, out var connectedDispenser))
                    {
                        if (connectedDispenser.EmoteToRoleBindings.TryGetValue(reaction.Emote, out var role))
                        {
                            await guildChannel.Guild.GetUser(reaction.UserId).AddRoleAsync(role);
                        }
                    }
                }
            }
        }
        private async Task ReactionRemovedHandler(Cacheable<IUserMessage, ulong> messageCache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.IsSpecified && !reaction.User.Value.IsBot)
            {
                if (channel is SocketGuildChannel guildChannel)
                {
                    if (_groupHandlingService.TryGetGroup(guildChannel.Guild.Id, reaction.MessageId, out var connectedGroup))
                    {
                        if (reaction.Emote.Name != _config.Configuration["Emojis:DefaultCallEmoji"] && reaction.Emote.Name != _config.Configuration["Emojis:DefaultCloseEmoji"])
                        {
                            var userList = connectedGroup.UserLists.FirstOrDefault(x => x.JoinEmote.Name == reaction.Emote.Name);
                            if (userList != null)
                            {
                                var userToRemove = userList.Users.FirstOrDefault(x => x.Id == reaction.UserId);
                                userList.Users.Remove(userToRemove);
                                await _groupHandlingService.ReplaceWithNewerGroup(connectedGroup);
                            }
                        }
                    }
                    else if (_roleDispenserService.TryGetRoleDispenser(guildChannel.Guild.Id, reaction.MessageId, out var connectedDispenser))
                    {
                        if (connectedDispenser.EmoteToRoleBindings.TryGetValue(reaction.Emote, out var role))
                        {
                            await guildChannel.Guild.GetUser(reaction.UserId).RemoveRoleAsync(role);
                        }
                    }
                }
            }
        }
    }
}
