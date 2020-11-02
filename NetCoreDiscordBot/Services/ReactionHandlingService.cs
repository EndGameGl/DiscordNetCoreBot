using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services
{
    public class ReactionHandlingService
    {
        private readonly GuildDataExtensionsService _dataExtensionsService;
        private readonly DiscordSocketClient _discordClient; 
        private readonly IServiceProvider _services;
        private readonly GroupHandlingService _groupHandlingService;
        private readonly RoleDispenserService _roleDispenserService;
        public ReactionHandlingService(IServiceProvider services)
        {
            _discordClient = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _groupHandlingService = services.GetRequiredService<GroupHandlingService>();
            _roleDispenserService = services.GetRequiredService<RoleDispenserService>();
            _dataExtensionsService = services.GetRequiredService<GuildDataExtensionsService>();

            _discordClient.ReactionAdded += ReactionAddedHandler;
            _discordClient.ReactionRemoved += ReactionRemovedHandler;
        }

        private async Task ReactionAddedHandler(Cacheable<IUserMessage, ulong> messageCache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.IsSpecified && !reaction.User.Value.IsBot)
            {
                if (channel is SocketGuildChannel guildChannel)
                {
                    if (_groupHandlingService.IsGroup(guildChannel.Guild, reaction.MessageId))
                    {
                        var connectedGroup = _groupHandlingService.GuildGroupLists[guildChannel.Guild.Id].FirstOrDefault(x => x.PresentationMessage.Id == reaction.MessageId);
                        if (connectedGroup != null)
                        {

                            if (reaction.Emote.Name == Core.Configuration["Emojis:DefaultCloseEmoji"])
                            {
                                if (reaction.UserId == connectedGroup.Host.Id || _dataExtensionsService.CheckIfAdmin(connectedGroup.Guild.Id, reaction.UserId))
                                {
                                    await connectedGroup.CloseMessage();
                                    await _groupHandlingService.RemoveGroup(connectedGroup);
                                }
                            }
                            else if (reaction.Emote.Name == Core.Configuration["Emojis:DefaultCallEmoji"])
                            {
                                if (reaction.UserId == connectedGroup.Host.Id || _dataExtensionsService.CheckIfAdmin(connectedGroup.Guild.Id, reaction.UserId))
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
                                        await _groupHandlingService.UpdateGroup(connectedGroup);
                                    }
                                    else
                                        await connectedGroup.PresentationMessage.RemoveReactionAsync(reaction.Emote, user);
                                }
                            }
                        }
                    }
                    else if (_roleDispenserService.IsDispenser(guildChannel.Guild, reaction.MessageId))
                    {
                        var connectedDispenser = _roleDispenserService.GuildDispensers[guildChannel.Guild.Id].FirstOrDefault(x => x.ListenedMessage.Id == reaction.MessageId);
                        if (connectedDispenser != null)
                        {
                            if (connectedDispenser.EmoteToRoleBindings.TryGetValue(reaction.Emote, out var role))
                            {
                                await guildChannel.Guild.GetUser(reaction.UserId).AddRoleAsync(role);
                            }
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
                    if (_groupHandlingService.IsGroup(guildChannel.Guild, reaction.MessageId))
                    {
                        if (reaction.Emote.Name != Core.Configuration["Emojis:DefaultCallEmoji"] && reaction.Emote.Name != Core.Configuration["Emojis:DefaultCloseEmoji"])
                        {
                            var connectedGroup = _groupHandlingService.GuildGroupLists[guildChannel.Guild.Id].FirstOrDefault(x => x.PresentationMessage.Id == reaction.MessageId);
                            var userList = connectedGroup.UserLists.FirstOrDefault(x => x.JoinEmote.Name == reaction.Emote.Name);
                            if (userList != null)
                            {
                                var userToRemove = userList.Users.FirstOrDefault(x => x.Id == reaction.UserId);
                                userList.Users.Remove(userToRemove);
                                await _groupHandlingService.UpdateGroup(connectedGroup);
                            }
                        }
                    }
                    else if (_roleDispenserService.IsDispenser(guildChannel.Guild, reaction.MessageId))
                    {
                        var connectedDispenser = _roleDispenserService.GuildDispensers[guildChannel.Guild.Id].FirstOrDefault(x => x.ListenedMessage.Id == reaction.MessageId);
                        if (connectedDispenser != null)
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
}
