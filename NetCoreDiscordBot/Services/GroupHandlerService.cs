using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NetCoreDiscordBot.Models.Groups;
using NetCoreDiscordBot.Models.Groups.References;
using NetCoreDiscordBot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services
{
    public class GroupHandlerService : IGroupHandlerService
    {
        private readonly ILoggerService _logger;
        private readonly DiscordSocketClient _discordClient;

        private readonly IMongoCollection<GroupReference> _groups;
        private Dictionary<ulong, List<Group>> _guildGroups;

        public GroupHandlerService(IServiceProvider services)
        {
            var config = services.GetRequiredService<IConfigurationService>();
            var collectionName = config.Configuration.GetSection("MongoDB:GroupsDatabase").Value;

            _logger = services.GetRequiredService<ILoggerService>();
            _discordClient = services.GetRequiredService<DiscordSocketClient>();

            _groups = services.GetRequiredService<IMongoDBAccessService>().GetCollection<GroupReference>(collectionName);

            _guildGroups = new Dictionary<ulong, List<Group>>();
        }
        public async Task InitializeAsync()
        {
            foreach (var guild in _discordClient.Guilds)
            {
                if (!_guildGroups.ContainsKey(guild.Id))
                    _guildGroups.Add(guild.Id, new List<Group>());
            }
            await LoadAllGroups();
        }
        public async Task AddGroup(Group group)
        {
            _guildGroups[group.Guild.Id].Add(group);
            await _groups.InsertOneAsync(new GroupReference(group));
        }
        public async Task ReplaceWithNewerGroup(Group group)
        {
            await _groups.ReplaceOneAsync(x => x.GUID == group.Guid, new GroupReference(group));
            await group.UpdateMessage();
        }
        public async Task RemoveGroup(Group group)
        {
            await _groups.DeleteOneAsync(x => x.GUID == group.Guid);
            _guildGroups[group.Guild.Id].Remove(group);
        }
        private async Task LoadAllGroups()
        {
            foreach (var guild in _discordClient.Guilds)
            {
                foreach (var reference in _groups.Find(x => x.GuildId == guild.Id).ToEnumerable())
                {
                    var loadedGroup = await LoadGroupFromReference(reference);
                    _guildGroups[guild.Id].Add(loadedGroup);
                }
            }
        }
        private async Task<Group> LoadGroupFromReference(GroupReference groupReference)
        {
            var guild = _discordClient.GetGuild(groupReference.GuildId);
            if (guild == null)
                throw new Exception("Missing guild.");
            var host = guild.GetUser(groupReference.HostId);
            if (host == null)
                throw new Exception("Missing host user.");
            var channel = guild.GetTextChannel(groupReference.ChannelId);
            if (channel == null)
                throw new Exception("Missing channel.");
            RestUserMessage message = null;
            if (groupReference.MessageId.HasValue)
            {
                var downloadedMessage = await channel.GetMessageAsync(groupReference.MessageId.Value);
                if (downloadedMessage == null)
                    throw new Exception();
                message = (RestUserMessage)downloadedMessage;
            }
            List<GroupUserList> userLists = new List<GroupUserList>();
            int i = 0;
            foreach (var userList in groupReference.UserListRefences)
            {
                List<SocketGuildUser> users = new List<SocketGuildUser>();
                foreach (var id in userList.UserIds)
                {
                    if (guild.Users.Any(x => x.Id == id))
                    {
                        var nextUser = guild.GetUser(id);
                        if (nextUser != null)
                            users.Add(nextUser);
                    }
                }
                userLists.Add(new GroupUserList()
                {
                    Description = userList.Description,
                    JoinEmote = new Emoji(userList.Emote),
                    UserLimit = userList.UserLimit,
                    Users = users
                });
            }
            return new Group()
            {
                Guid = groupReference.GUID,
                Guild = guild,
                Host = host,
                Channel = channel,
                Description = groupReference.Description,
                CreatedAt = groupReference.CreatedAt,
                PresentationMessage = message,
                UserLists = userLists.ToList()
            };
        }
        public bool IsGroup(SocketGuild guild, ulong messageId)
        {
            return _guildGroups[guild.Id].Any(x => x.PresentationMessage.Id == messageId);
        }
        public bool TryGetGroup(ulong guildId, Guid guid, out Group group)
        {
            group = default;
            if (_guildGroups.TryGetValue(guildId, out var groupList))
            {
                group = groupList.FirstOrDefault(x => x.Guid == guid);
                return group != null;
            }
            else
                return false;
        }
        public bool TryGetGroup(ulong guildId, ulong messageId, out Group group)
        {
            group = default;
            if (_guildGroups.TryGetValue(guildId, out var groupList))
            {
                group = groupList.FirstOrDefault(x => x.PresentationMessage.Id == messageId);
                return group != null;
            }
            else
                return false;
        }
        public bool TryGetGuildGroups(ulong guildId, out IEnumerable<Group> groups)
        {
            groups = default;
            if (_guildGroups.TryGetValue(guildId, out var groupList))
            {
                groups = groupList.AsEnumerable();
                return true;
            }
            else
                return false;
        }
    }
}
