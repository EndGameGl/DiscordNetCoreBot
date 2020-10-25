using Discord.Commands;
using NetCoreDiscordBot.Services;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Modules.Commands
{
    public class InfoModule : ModuleBase<ShardedCommandContext>
    {
        private GuildDataExtensionsService _guildDataExtensionsService { get; }
        private CommandHandlingService _commandsService { get; }
        public InfoModule(GuildDataExtensionsService guildDataExtensionsService, CommandHandlingService commandsService)
        {
            _guildDataExtensionsService = guildDataExtensionsService;
            _commandsService = commandsService;
        }

        [Command("Guild settings")]
        public async Task GetGuildSettings()
        {
            var guildExtendedData = _guildDataExtensionsService.ExtendedGuilds[Context.Guild];           
            await ReplyAsync($"```{JsonConvert.SerializeObject(guildExtendedData, new JsonSerializerSettings() { Formatting = Formatting.Indented })}```");
        }

        [Command("help")]
        public async Task GetHelp()
        {
            StringBuilder stringBuilder = new StringBuilder();
            var commandsData = _commandsService.Commands.Commands;
            foreach (var command in commandsData)
            {
                stringBuilder.AppendLine($"> __{command.Name}__: {command.Summary}\n    `_{command.Module?.Name} {command.Name} {string.Join(' ', command.Parameters.Select(x => $"[{x.Name}]"))}`\n");
            }
            await ReplyAsync(stringBuilder.ToString());
        }
    }
}
