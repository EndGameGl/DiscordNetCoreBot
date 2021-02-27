using Discord.Commands;
using NetCoreDiscordBot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BungieNetCoreAPI.Destiny.Definitions;
using BungieNetCoreAPI;
using BungieNetCoreAPI.Destiny;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace NetCoreDiscordBot.Modules.Commands
{
    [Group("destiny")]
    public class BungieModule : ModuleBase<SocketCommandContext>
    {
        private readonly IBungieService _service;
        public BungieModule(IBungieService service)
        {
            _service = service;
        }

        [Command("fetch json")]
        public async Task FetchDefinitionJson(string locale, string defType, uint hash)
        {
            DestinyLocales parsedLocale;
            DefinitionsEnum definitionType;
            if (Enum.TryParse(typeof(DestinyLocales), locale, true, out var localeParseAttempt))
            {
                parsedLocale = (DestinyLocales)localeParseAttempt;
            }
            else
            {
                await ReplyAsync("Couldn't parse locale");
                return;
            }
            if (Enum.TryParse(typeof(DefinitionsEnum), $"Destiny{defType}Definition", true, out var defTypeParseAttempt))
            {
                definitionType = (DefinitionsEnum)defTypeParseAttempt;
            }
            else
            {
                await ReplyAsync("Couldn't parse definition type");
                return;
            }

            try
            {
                var result = _service.Client.Repository.FetchJSONFromDB(parsedLocale, definitionType, hash);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    if (result.Length <= 2000)
                    {
                        JToken token = JToken.Parse(result);
                        var indentedResult = token.ToString(Formatting.Indented);
                        if (indentedResult.Length <= 2000)
                        {
                            await ReplyAsync($"```{indentedResult}```");
                        }
                        else
                        {
                            await ReplyAsync($"```{indentedResult.Substring(0, 1989)}\n...```");
                        }
                    }
                    else
                        await ReplyAsync("Definition is too long to display.");
                }
                else
                    await ReplyAsync("Error while fetching json.");
            }
            catch
            {
                await ReplyAsync("Error while fetching json.");
            }
        }
    }
}
