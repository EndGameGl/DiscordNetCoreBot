using NetCoreDiscordBot.Models.Dispensers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services.Interfaces
{
    public interface IRoleDispenserService
    {
        Task InitializeAsync();
        Task AddRoleDispenser(RoleDispenser dispenser);
        Task ReplaceWithNewerRoleDispenser(RoleDispenser dispenser);
        Task RemoveRoleDispenser(RoleDispenser dispenser);
        bool TryGetRoleDispenser(ulong guildId, Guid guid, out RoleDispenser dispenser);
        bool TryGetRoleDispenser(ulong guildId, ulong messageId, out RoleDispenser dispenser);
        bool TryGetGuildRoleDispensers(ulong guildId, out IEnumerable<RoleDispenser> dispensers);
    }
}
