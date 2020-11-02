using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NetCoreDiscordBot.Models.Dispensers;
using NetCoreDiscordBot.Models.Dispensers.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services
{
    public class RoleDispenserService
    {
        private readonly RoleDispensersContext _database;
        private readonly DiscordSocketClient _discordClient;
        public Dictionary<ulong, List<RoleDispenser>> GuildDispensers { get; set; }
        public RoleDispenserService(IServiceProvider services)
        {
            _database = services.GetRequiredService<RoleDispensersContext>();
            _discordClient = services.GetRequiredService<DiscordSocketClient>();
            GuildDispensers = new Dictionary<ulong, List<RoleDispenser>>();
        }
        public async Task InitializeAsync()
        {
            foreach (var guild in _discordClient.Guilds)
            {
                if (!GuildDispensers.ContainsKey(guild.Id))
                    GuildDispensers.Add(guild.Id, new List<RoleDispenser>());
            }
        }
        public async Task AddDispenserAndSaveToDataBase(RoleDispenser dispenser)
        {
            GuildDispensers[dispenser.Guild.Id].Add(dispenser);
            _database.DispenseresReferences.Add(new RoleDispenserReference(dispenser));
            await _database.SaveChangesAsync();
        }
        public async Task UpdateDispenser(RoleDispenser dispenser)
        {
            var reference = _database.DispenseresReferences.FirstOrDefault(x => x.Guid == dispenser.Guid);
            reference.Update(dispenser);
            _database.DispenseresReferences.Update(reference);
            await _database.SaveChangesAsync();
            await dispenser.UpdateMessage();
        }
        public async Task RemoveDispenser(RoleDispenser dispenser)
        {
            _database.DispenseresReferences.Remove(_database.DispenseresReferences.FirstOrDefault(x => x.Guid == dispenser.Guid));
            await _database.SaveChangesAsync();
            GuildDispensers[dispenser.Guild.Id].Remove(dispenser);
        }
        public async Task LoadAllDispensers()
        {
            foreach (var guild in _discordClient.Guilds)
            {
                var references = await _database.GetDispenserReferences(guild.Id);
                foreach (var reference in references)
                {
                    GuildDispensers[guild.Id].Add(await LoadDispenserFromReference(reference));
                }
            }
        }
        private async Task<RoleDispenser> LoadDispenserFromReference(RoleDispenserReference dispenserReference)
        {
            var guild = _discordClient.GetGuild(dispenserReference.GuildId);
            var channel = guild.GetTextChannel(dispenserReference.ChannelId);
            RestUserMessage message = null;
            if (dispenserReference.MessageId.HasValue)
            {
                var downloadedMessage = await channel.GetMessageAsync(dispenserReference.MessageId.Value);
                if (downloadedMessage == null)
                    throw new Exception();
                message = (RestUserMessage)downloadedMessage;
            }
            Dictionary<IEmote, IRole> bindings = new Dictionary<IEmote, IRole>();
            foreach (var pair in dispenserReference.Bindings)
            {
                bindings.Add(new Emoji(pair.Emote), guild.GetRole(pair.RoleId));
            }
            return new RoleDispenser()
            {
                Guid = dispenserReference.Guid,
                Guild = guild,
                Channel = channel,
                Description = dispenserReference.Description,
                ListenedMessage = message,
                EmoteToRoleBindings = bindings
            };
        }
        public bool IsDispenser(SocketGuild guild, ulong messageId)
        {
            return GuildDispensers[guild.Id].Any(x => x.ListenedMessage.Id == messageId);
        }
    }
}
