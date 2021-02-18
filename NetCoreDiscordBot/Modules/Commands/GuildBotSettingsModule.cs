using Discord.Commands;
using Discord.WebSocket;
using NetCoreDiscordBot.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Modules.Commands
{
    [Group("settings")]
    [RequireContext(ContextType.Guild)]
    public class GuildBotSettingsModule : ModuleBase<SocketCommandContext>
    {
        private IGuildDataExtensionsService _guildDataExtensionsService { get; }
        public GuildBotSettingsModule(IGuildDataExtensionsService guildDataExtensionsService)
        {
            _guildDataExtensionsService = guildDataExtensionsService;
        }

        [Command("set prefix")]
        public async Task SetGuildPrefix(char newPrefix)
        {
            if (_guildDataExtensionsService.TryGetData(Context.Guild.Id, out var data))
            {
                data.CommandPrefix = newPrefix;
                await _guildDataExtensionsService.SaveGuildData(Context.Guild.Id);
                await ReplyAsync($"Changed prefix for this server to: {data.CommandPrefix}");
            }
        }

        [Command("set api token")]
        public async Task SetAPIToken(string newToken)
        {
            if (_guildDataExtensionsService.TryGetData(Context.Guild.Id, out var data))
            {
                data.APIToken = newToken;
                await _guildDataExtensionsService.SaveGuildData(Context.Guild.Id);
                await ReplyAsync($"Changed prefix for this server to: {data.APIToken}");
            }
        }

        [Command("add bot admin")]        
        public async Task AddBotAdmin(SocketGuildUser newAdmin)
        {
            if (_guildDataExtensionsService.TryGetData(Context.Guild.Id, out var data))
            {
                var admins = data.Admins;
                if (!admins.Any(x => x == newAdmin.Id))
                {
                    admins.Add(newAdmin.Id);
                    await _guildDataExtensionsService.SaveGuildData(Context.Guild.Id);
                    await ReplyAsync($"Added user {newAdmin.Mention} as new bot admin!");
                }
                else
                {
                    await ReplyAsync($"This user is already admin.");
                }
            }                         
        }

        [Command("remove bot admin")]
        public async Task RemoveBotAdmin(SocketGuildUser newAdmin)
        {
            if (_guildDataExtensionsService.TryGetData(Context.Guild.Id, out var data))
            {
                var admins = data.Admins;
                if (admins.Any(x => x == newAdmin.Id))
                {
                    admins.Remove(newAdmin.Id);
                    await _guildDataExtensionsService.SaveGuildData(Context.Guild.Id);
                    await ReplyAsync($"Removed user {newAdmin.Mention} from bot admins.");
                }
                else
                {
                    await ReplyAsync($"This user is not admin.");
                }
            }
        }

    }
}
