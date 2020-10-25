using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace NetCoreDiscordBot.Models.Groups.References
{
    [Owned]
    public class GroupUserListRefence
    {
        public string Description { get; set; }
        public string Emote { get; set; }
        public List<GroupUserIdReference> UserIds { get; set; }
        public int? UserLimit { get; set; }
        private GroupUserListRefence() { }
        public GroupUserListRefence(GroupUserList groupUserList)
        {
            Description = groupUserList.Description;
            Emote = groupUserList.JoinEmote.Name;
            UserIds = new List<GroupUserIdReference>();
            for (int i = 0; i < groupUserList.Users.Count; i++)
            {
                UserIds.Add(new GroupUserIdReference() { Value = groupUserList.Users[i].Id });
            }
            UserLimit = groupUserList.UserLimit;
        }
    }
}
