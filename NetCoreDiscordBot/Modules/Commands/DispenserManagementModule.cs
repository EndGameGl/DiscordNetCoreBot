﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NetCoreDiscordBot.Models.Dispensers;
using NetCoreDiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Modules.Commands
{
    [Group("dispenser")]
    [RequireContext(ContextType.Guild)]
    public class DispenserManagementModule : ModuleBase<ShardedCommandContext>
    {
        private RoleDispenserService _service { get; set; }
        public DispenserManagementModule(RoleDispenserService service)
        {
            _service = service;
        }

        [Command("create")]
        public async Task CreateDispenser(params string[] description)
        {
            RoleDispenser dispenser = new RoleDispenser(Context.Guild, (SocketTextChannel)Context.Channel, string.Join(' ', description));
            await dispenser.SendMessage();
            await _service.AddDispenserAndSaveToDataBase(dispenser);
        }
        [Command("add")]
        public async Task AddBinding(ulong messageId, string emojiText, IRole role)
        {
            Emoji emoji = new Emoji(emojiText);
            var dispenser = _service.GuildDispensers[Context.Guild].FirstOrDefault(x => x.ListenedMessage.Id == messageId);
            if (dispenser != null)
            {
                if (dispenser.TryAddNewBinding(emoji, role))
                {
                    await _service.UpdateDispenser(dispenser);
                    await dispenser.ListenedMessage.AddReactionAsync(emoji);
                }
                else
                    await ReplyAsync("Уже есть такая реакция.");
            }
        }
        [Command("remove")]
        public async Task RemoveBinding(ulong messageId, string emojiText)
        {
            Emoji emoji = new Emoji(emojiText);
            var dispenser = _service.GuildDispensers[Context.Guild].FirstOrDefault(x => x.ListenedMessage.Id == messageId);
            if (dispenser != null)
            {
                if (dispenser.TryRemoveBinding(emoji))
                {
                    await _service.UpdateDispenser(dispenser);
                    await dispenser.ListenedMessage.RemoveReactionAsync(emoji, Context.Client.CurrentUser);
                }
                else
                    await ReplyAsync("Такой привязки нет.");
            }
        }

        [Command("delete")]
        public async Task DeleteDispenser(ulong messageId)
        {
            var dispenser = _service.GuildDispensers[Context.Guild].FirstOrDefault(x => x.ListenedMessage?.Id == messageId);
            if (dispenser != null)
            {
                await _service.RemoveDispenser(dispenser);
                await dispenser.ListenedMessage.DeleteAsync();
            }
        }
    }
}