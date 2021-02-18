using NetCoreDiscordBot.Models.Guilds;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services.Interfaces
{
    public interface IGuildDataExtensionsService
    {
        Task InitializeAsync();
        bool TryGetData(ulong guildId, out GuildDataExtension data);
        Task SaveGuildData(ulong guildId);
        bool IsAdmin(ulong guildId, ulong userId);
    }
}
