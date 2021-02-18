using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Modules.Commands
{
    public class UtilitiesModule : ModuleBase<SocketCommandContext>
    {
        public UtilitiesModule() { }

        [Command("clear"), RequireOwner]
        public async Task ClearMessagesRange(int amount)
        {
            var messages = await Context.Channel.GetMessagesAsync(amount + 1, CacheMode.AllowDownload).FlattenAsync();
            foreach (var mes in messages)
            {
                if ((DateTime.Now - mes.CreatedAt.Date).Days > 14)
                {
                    await ReplyAsync("You are trying to delete messages older than 2 weeks.");
                    return;
                }
            }
            await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(messages);
            await ReplyAsync($"Deleted {amount} messages");
        }
    }
}
