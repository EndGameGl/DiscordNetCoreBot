using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NetCoreDiscordBot.Models.Groups;
using NetCoreDiscordBot.Models.Groups.References;
using NetCoreDiscordBot.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Modules.Commands
{
    [Group("group")]
    [RequireContext(ContextType.Guild)]
    public class GroupManagementModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationService _config;
        private readonly IGroupHandlerService _groupService;
        public GroupManagementModule(IGroupHandlerService groupService, IConfigurationService config)
        {
            _groupService = groupService;
            _config = config;
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
                        new GroupUserList(
                            description: "Group members",
                            joinEmote: new Emoji(_config.Configuration["Emojis:DefaultJoinEmoji"]),
                            userLimit: numberOfSlots)
                    },
                    description: string.Join(' ', description),
                    type: GroupType.Simple);
            await group.SendMessage(_config.Configuration["Emojis:DefaultCallEmoji"], _config.Configuration["Emojis:DefaultCloseEmoji"]);
            await _groupService.AddGroup(group);
        }

        [Command("remove"), Summary("Removes group with specified GUID"), RequireOwner]
        public async Task RemoveGroup(string groupGuid)
        {
            Guid guid = Guid.Parse(groupGuid);
            if (_groupService.TryGetGroup(Context.Guild.Id, guid, out var group))
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
            if (_groupService.TryGetGuildGroups(Context.Guild.Id, out var groups))
            {
                var message = "Current groups at this guild:\n";
                foreach (var group in groups)
                {
                    message += $"{group.ToString()}\n";
                }
                await ReplyAsync($"{message}");
            }
        }

        [Command("edit descr"), Summary("Edits description of group")]
        public async Task EditGroupDescription(string groupGuid, params string[] newDescription)
        {
            Guid guid = Guid.Parse(groupGuid);
            if (_groupService.TryGetGroup(Context.Guild.Id, guid, out var group))
            {
                if (group.Host.Id != Context.User.Id)
                    return;

                group.Description = (newDescription.Length > 0) ? (string.Join(' ', newDescription)) : string.Empty;
                await _groupService.ReplaceWithNewerGroup(group);
                await ReplyAsync("Group description was updated!");
            }
            else
                await ReplyAsync("No groups with such ID were found");
        }

        [Command("get as"), Summary("Gets group-specific data")]
        public async Task GetGUIDFromID(string type, ulong id)
        {
            if (_groupService.TryGetGroup(Context.Guild.Id, id, out var group))
            {
                string reply = (type.ToUpper()) switch
                {
                    "GUID" => group.Guid.ToString(),
                    "JSON" => $"```{JsonConvert.SerializeObject(new GroupReference(group), Formatting.Indented)}```",
                    _ => "Wrong type. Should be \"GUID\" or \"JSON\"",
                };
                await ReplyAsync(reply);
            }
        }

        [Command("add choice"), Summary("Adds new join choice for group")]
        public async Task AddUserList(string groupGuid, string emoji, int? amount, params string[] description)
        {
            Guid guid = Guid.Parse(groupGuid);
            if (_groupService.TryGetGroup(Context.Guild.Id, guid, out var group))
            {
                if (group.Host.Id != Context.User.Id)
                    return;

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
                    await _groupService.ReplaceWithNewerGroup(group);
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

            if (_groupService.TryGetGroup(Context.Guild.Id, guid, out var group))
            {

                if (group.Host.Id != Context.User.Id)
                    return;

                var userList = group.UserLists.FirstOrDefault(x => x.JoinEmote.Name == emoji);
                group.UserLists.Remove(userList);
                await _groupService.ReplaceWithNewerGroup(group);
            }
            else
                await ReplyAsync("No groups with such ID were found");
        }
    }
}
