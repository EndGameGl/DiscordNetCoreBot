using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NetCoreDiscordBot.Models.Dispensers;
using NetCoreDiscordBot.Services.Interfaces;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Modules.Commands
{
    [Group("dispenser")]
    [RequireContext(ContextType.Guild)]
    public class DispenserManagementModule : ModuleBase<SocketCommandContext>
    {
        private IRoleDispenserService _service { get; set; }
        public DispenserManagementModule(IRoleDispenserService service)
        {
            _service = service;
        }

        [Command("create"), RequireOwner]
        public async Task CreateDispenser(params string[] description)
        {
            RoleDispenser dispenser = new RoleDispenser(Context.Guild, (SocketTextChannel)Context.Channel, string.Join(' ', description));
            await dispenser.SendMessage();
            await _service.AddRoleDispenser(dispenser);
        }
        [Command("add"), RequireOwner]
        public async Task AddBinding(ulong messageId, string emojiText, IRole role)
        {
            Emoji emoji = new Emoji(emojiText);
            if (_service.TryGetRoleDispenser(Context.Guild.Id, messageId, out var dispenser))
            {
                await _service.ReplaceWithNewerRoleDispenser(dispenser);
                await dispenser.ListenedMessage.AddReactionAsync(emoji);
            }
            else
                await ReplyAsync("Уже есть такая реакция.");
        }
        [Command("remove"), RequireOwner]
        public async Task RemoveBinding(ulong messageId, string emojiText)
        {
            Emoji emoji = new Emoji(emojiText);
            if (_service.TryGetRoleDispenser(Context.Guild.Id, messageId, out var dispenser))
            {
                await _service.ReplaceWithNewerRoleDispenser(dispenser);
                await dispenser.ListenedMessage.RemoveReactionAsync(emoji, Context.Client.CurrentUser);
            }
            else
                await ReplyAsync("Такой привязки нет.");
        }

        [Command("delete"), RequireOwner]
        public async Task DeleteDispenser(ulong messageId)
        {
            if (_service.TryGetRoleDispenser(Context.Guild.Id, messageId, out var dispenser))
            {
                await _service.RemoveRoleDispenser(dispenser);
                await dispenser.ListenedMessage.DeleteAsync();
            }
        }
    }
}
