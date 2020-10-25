using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NetCoreDiscordBot.Models.Groups;
using NetCoreDiscordBot.Models.Groups.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services
{
    public class GroupHandlingService
    {
        private readonly DebugService _debugService;
        private readonly DiscordShardedClient _discordClient;
        private readonly GroupsContext dataBase;
        public Dictionary<SocketGuild, List<Group>> GuildGroupLists { get; set; }
        public GroupHandlingService(IServiceProvider services)
        {
            _debugService = services.GetRequiredService<DebugService>();
            _discordClient = services.GetRequiredService<DiscordShardedClient>();
            dataBase = services.GetRequiredService<GroupsContext>();
            GuildGroupLists = new Dictionary<SocketGuild, List<Group>>();
        }
        public async Task InitializeAsync()
        {
            foreach (var guild in _discordClient.Guilds)
            {
                GuildGroupLists.Add(guild, new List<Group>());
            }
        }
        public async Task AddGroupAndSaveToDataBase(Group group)
        {
            GuildGroupLists[group.Guild].Add(group);
            dataBase.Groups.Add(new GroupReference(group));
            await dataBase.SaveChangesAsync();
        }
        public async Task UpdateGroup(Group group)
        {
            var reference = dataBase.Groups.FirstOrDefault(x => x.GUID == group.Guid);
            reference.Update(group);
            dataBase.Groups.Update(reference);
            await dataBase.SaveChangesAsync();
            await group.UpdateMessage();
        }
        public async Task RemoveGroup(Group group)
        {
            dataBase.Groups.Remove(dataBase.Groups.FirstOrDefault(x => x.GUID == group.Guid));
            await dataBase.SaveChangesAsync();
            GuildGroupLists[group.Guild].Remove(group);
        }
        public async Task LoadAllGroups()
        {
            foreach (var guild in _discordClient.Guilds)
            {
                var references = await dataBase.GetGroupReferences(guild.Id);
                foreach (var reference in references)
                {
                    _debugService.Log($"Loading group {reference.GUID}");
                    GuildGroupLists[guild].Add(await LoadGroupFromReference(reference));
                    _debugService.Log($"Group {reference.GUID} loaded");
                }
            }
        }
        private async Task<Group> LoadGroupFromReference(GroupReference groupReference)
        {
            var guild = _discordClient.GetGuild(groupReference.GuildId);
            var host = guild.GetUser(groupReference.HostId);
            var channel = guild.GetTextChannel(groupReference.ChannelId);
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
                    if (guild.Users.Any(x => x.Id == id.Value))
                    {
                        var nextUser = guild.GetUser(id.Value);
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
            return GuildGroupLists[guild].Any(x => x.PresentationMessage.Id == messageId);
        }
    }
}
