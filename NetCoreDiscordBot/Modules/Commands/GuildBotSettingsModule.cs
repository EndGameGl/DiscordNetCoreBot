using Discord.Commands;
using Discord.WebSocket;
using NetCoreDiscordBot.Models.Guilds;
using NetCoreDiscordBot.Services;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Modules.Commands
{
    [Group("settings")]
    [RequireContext(ContextType.Guild)]
    public class GuildBotSettingsModule : ModuleBase<ShardedCommandContext>
    {
        private GuildDataExtensionsService _guildDataExtensionsService { get; }
        public GuildBotSettingsModule(GuildDataExtensionsService guildDataExtensionsService)
        {
            _guildDataExtensionsService = guildDataExtensionsService;
        }

        [Command("set prefix")]
        public async Task SetGuildPrefix(char newPrefix)
        {
            _guildDataExtensionsService.ExtendedGuilds[Context.Guild].CommandPrefix = newPrefix;
            await _guildDataExtensionsService.ForceSaveDb();
            await ReplyAsync($"Changed prefix for this server to: {_guildDataExtensionsService.ExtendedGuilds[Context.Guild].CommandPrefix}");
        }

        [Command("set api token")]
        public async Task SetAPIToken(string newToken)
        {
            _guildDataExtensionsService.ExtendedGuilds[Context.Guild].APIToken = newToken;
            await _guildDataExtensionsService.ForceSaveDb();
            await ReplyAsync($"Changed prefix for this server to: {_guildDataExtensionsService.ExtendedGuilds[Context.Guild].APIToken}");
        }

        [Command("add bot admin")]        
        public async Task AddBotAdmin(SocketGuildUser newAdmin)
        {
            var admins = _guildDataExtensionsService.ExtendedGuilds[Context.Guild].Admins;
            if (!admins.Any(x => x.AdminId == newAdmin.Id))
            {
                admins.Add(new GuildDataExtensionAdminEntry() { AdminId = newAdmin.Id });
                await _guildDataExtensionsService.ForceSaveDb();
                await ReplyAsync($"Added user {newAdmin.Mention} as new bot admin!");
            }
            else
            {
                await ReplyAsync($"This user is already admin.");
            }            
        }

        [Command("remove bot admin")]
        public async Task RemoveBotAdmin(SocketGuildUser newAdmin)
        {
            var admins = _guildDataExtensionsService.ExtendedGuilds[Context.Guild].Admins;
            if (admins.Any(x => x.AdminId == newAdmin.Id))
            {
                var admin = admins.FirstOrDefault(x => x.AdminId == newAdmin.Id);
                admins.Remove(admin);
                await _guildDataExtensionsService.ForceSaveDb();
                await ReplyAsync($"Removed user {newAdmin.Mention} from bot admins.");
            }
            else
            {
                await ReplyAsync($"This user is not admin.");
            }
        }

    }
}
