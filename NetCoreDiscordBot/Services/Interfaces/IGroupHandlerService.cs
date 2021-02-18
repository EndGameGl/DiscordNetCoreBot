using NetCoreDiscordBot.Models.Groups;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services.Interfaces
{
    public interface IGroupHandlerService
    {
        Task InitializeAsync();
        Task AddGroup(Group group);
        Task ReplaceWithNewerGroup(Group group);
        Task RemoveGroup(Group group);
        bool TryGetGroup(ulong guildId, Guid guid, out Group group);
        bool TryGetGroup(ulong guildId, ulong messageId, out Group group);
        bool TryGetGuildGroups(ulong guildId, out IEnumerable<Group> groups);
    }
}
