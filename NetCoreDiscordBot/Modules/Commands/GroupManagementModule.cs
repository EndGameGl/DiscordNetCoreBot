using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NetCoreDiscordBot.Models.Groups;
using NetCoreDiscordBot.Models.Groups.References;
using NetCoreDiscordBot.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Modules.Commands
{
    [Group("group")]
    [RequireContext(ContextType.Guild)]
    public class GroupManagementModule : ModuleBase<ShardedCommandContext>
    {
        private readonly GroupHandlingService _groupService;
        public GroupManagementModule(GroupHandlingService groupService)
        {
            _groupService = groupService;
        }

        [Command("create"), Summary("Creates new group with given number of slots.")]
        public async Task CreateSimpleGroup(int numberOfSlots, params string[] description)
        {
            var group = new Group(
                    guild: Context.Guild,
                    host: (SocketGuildUser)Context.User,
                    channel: (SocketTextChannel)Context.Channel,
                    userLists: new List<GroupUserList>()
                    {
                        new GroupUserList("Group members", new Emoji(Core.Configuration["Emojis:DefaultJoinEmoji"]), numberOfSlots)
                    },
                    description: string.Join(' ', description),
                    type: GroupType.Simple
                    );
            await group.SendMessage();
            await _groupService.AddGroupAndSaveToDataBase(group);
        }

        [Command("remove"), Summary("Removes group with specified GUID"), RequireOwner]
        public async Task RemoveGroup(string groupGuid)
        {
            Guid guid = Guid.Parse(groupGuid);
            var group = _groupService.GuildGroupLists[Context.Guild].FirstOrDefault(x => x.Guid == guid);
            if (group != null)
            {
                await _groupService.RemoveGroup(group);
                await ReplyAsync("Group was removed successfully");
            }
            else
                await ReplyAsync("No groups with such ID were found");
        }

        [Command("list all"), Summary("Lists all groups in this guild")]
        public async Task ListGroups()
        {
            var groups = _groupService.GuildGroupLists[Context.Guild];
            var message = "Current groups at this guild:\n";
            foreach (var group in groups)
            {
                message += $"{group.ToString()}\n";
            }
            await ReplyAsync($"{message}");
        }

        [Command("edit descr"), Summary("Edits description of group")]
        public async Task EditGroupDescription(string groupGuid, params string[] newDescription)
        {
            Guid guid = Guid.Parse(groupGuid);
            var group = _groupService.GuildGroupLists[Context.Guild].FirstOrDefault(x => x.Guid == guid);

            if (group.Host.Id != Context.User.Id)
                return;

            if (group != null)
            {
                group.Description = (newDescription.Length > 0) ? (string.Join(' ', newDescription)) : string.Empty;
                await _groupService.UpdateGroup(group);
                await ReplyAsync("Group description was updated!");
            }
            else
                await ReplyAsync("No groups with such ID were found");
        }

        [Command("get as"), Summary("Gets group-specific data")]
        public async Task GetGUIDFromID(string type, ulong id)
        {
            var group = _groupService.GuildGroupLists[Context.Guild].FirstOrDefault(x => x.PresentationMessage.Id == id);
            string reply = string.Empty;
            switch (type.ToUpper())
            {
                case "GUID":
                    reply = group.Guid.ToString();
                    break;
                case "JSON":
                    reply = $"```{JsonConvert.SerializeObject(new GroupReference(group), Formatting.Indented)}```";
                    break;
            }
            await ReplyAsync(reply);
        }

        [Command("add choice"), Summary("Adds new join choice for group")]
        public async Task AddUserList(string groupGuid, string emoji, int? amount, params string[] description)
        {
            Guid guid = Guid.Parse(groupGuid);
            var group = _groupService.GuildGroupLists[Context.Guild].FirstOrDefault(x => x.Guid == guid);

            if (group.Host.Id != Context.User.Id)
                return;

            if (group != null)
            {
                if (!group.UserLists.Any(x => x.JoinEmote.Name == emoji))
                {
                    var newList = new GroupUserList()
                    {
                        Users = new List<SocketGuildUser>(),
                        JoinEmote = new Emoji(emoji),
                        UserLimit = amount,
                        Description = (description.Length > 0) ? string.Join(' ', description) : string.Empty
                    };
                    group.UserLists.Add(newList);
                    await _groupService.UpdateGroup(group);
                    await group.PresentationMessage.AddReactionAsync(newList.JoinEmote);
                }
                else
                    await ReplyAsync("There is already list with such emoji present.");
            }
            else
                await ReplyAsync("No groups with such ID were found");
        }

        [Command("remove choice"), Summary("Removes specified join choice of the group")]
        public async Task RemoveUserList(string groupGuid, string emoji)
        {
            Guid guid = Guid.Parse(groupGuid);
            var group = _groupService.GuildGroupLists[Context.Guild].FirstOrDefault(x => x.Guid == guid);

            if (group.Host.Id != Context.User.Id)
                return;

            if (group != null)
            {
                var userList = group.UserLists.FirstOrDefault(x => x.JoinEmote.Name == emoji);
                group.UserLists.Remove(userList);
                await _groupService.UpdateGroup(group);
            }
            else
                await ReplyAsync("No groups with such ID were found");
        }
    }
}
