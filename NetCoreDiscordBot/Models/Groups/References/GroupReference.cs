using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetCoreDiscordBot.Models.Groups.References
{
    public class GroupReference
    {
        [Key]
        public Guid GUID { get; set; }
        public ulong HostId { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public List<GroupUserListRefence> UserListRefences { get; set; }
        public ulong? MessageId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public GroupType Type { get; set; }
        private GroupReference() { }
        public GroupReference(Group group)
        {
            GUID = group.Guid;
            HostId = group.Host.Id;
            GuildId = group.Guild.Id;
            ChannelId = group.Channel.Id;
            UserListRefences = new List<GroupUserListRefence>();
            foreach (var userList in group.UserLists)
            {
                UserListRefences.Add(new GroupUserListRefence(userList)); 
            }
            MessageId = group.PresentationMessage?.Id;
            CreatedAt = group.CreatedAt;
            Description = group.Description;
            Type = group.Type;
        }

        public void Update(Group group)
        {
            GUID = group.Guid;
            HostId = group.Host.Id;
            GuildId = group.Guild.Id;
            ChannelId = group.Channel.Id;
            UserListRefences = new List<GroupUserListRefence>();
            foreach (var userList in group.UserLists)
            {
                UserListRefences.Add(new GroupUserListRefence(userList));
            }
            MessageId = group.PresentationMessage?.Id;
            CreatedAt = group.CreatedAt;
            Description = group.Description;
            Type = group.Type;
        }
    }
}
