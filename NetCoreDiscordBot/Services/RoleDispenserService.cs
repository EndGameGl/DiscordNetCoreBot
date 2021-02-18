using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NetCoreDiscordBot.Models.Dispensers;
using NetCoreDiscordBot.Models.Dispensers.References;
using NetCoreDiscordBot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services
{
    public class RoleDispenserService : IRoleDispenserService
    {
        private readonly ILoggerService _logger;
        private readonly DiscordSocketClient _discordClient;

        private readonly IMongoCollection<RoleDispenserReference> _roleDispensers;
        private Dictionary<ulong, List<RoleDispenser>> _guildDispensers;

        public RoleDispenserService(IServiceProvider services)
        {
            _discordClient = services.GetRequiredService<DiscordSocketClient>();
            _guildDispensers = new Dictionary<ulong, List<RoleDispenser>>();

            var config = services.GetRequiredService<IConfigurationService>();
            var collectionName = config.Configuration.GetSection("MongoDB:RoleDispensersDatabase").Value;

            _roleDispensers = services.GetRequiredService<IMongoDBAccessService>().GetCollection<RoleDispenserReference>(collectionName);
        }
        public async Task InitializeAsync()
        {
            foreach (var guild in _discordClient.Guilds)
            {
                if (!_guildDispensers.ContainsKey(guild.Id))
                    _guildDispensers.Add(guild.Id, new List<RoleDispenser>());
            }
            await LoadAllDispensers();
        }
        public async Task AddRoleDispenser(RoleDispenser dispenser)
        {
            _guildDispensers[dispenser.Guild.Id].Add(dispenser);
            await _roleDispensers.InsertOneAsync(new RoleDispenserReference(dispenser));
        }
        public async Task ReplaceWithNewerRoleDispenser(RoleDispenser dispenser)
        {
            await _roleDispensers.ReplaceOneAsync(x => x.Guid == dispenser.Guid, new RoleDispenserReference(dispenser));
            await dispenser.UpdateMessage();
        }
        public async Task RemoveRoleDispenser(RoleDispenser dispenser)
        {
            await _roleDispensers.DeleteOneAsync(x => x.Guid == dispenser.Guid);
            _guildDispensers[dispenser.Guild.Id].Remove(dispenser);
        }
        private async Task LoadAllDispensers()
        {
            foreach (var guild in _discordClient.Guilds)
            {
                foreach (var reference in _roleDispensers.Find(x => x.GuildId == guild.Id).ToEnumerable())
                {
                    _guildDispensers[guild.Id].Add(await LoadDispenserFromReference(reference));
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
        public bool TryGetRoleDispenser(ulong guildId, Guid guid, out RoleDispenser dispenser)
        {
            dispenser = default;
            if (_guildDispensers.TryGetValue(guildId, out var dispenserList))
            {
                dispenser = dispenserList.FirstOrDefault(x => x.Guid == guid);
                return dispenser != null;
            }
            else
                return false;
        }
        public bool TryGetRoleDispenser(ulong guildId, ulong messageId, out RoleDispenser dispenser)
        {
            dispenser = default;
            if (_guildDispensers.TryGetValue(guildId, out var dispenserList))
            {
                dispenser = dispenserList.FirstOrDefault(x => x.ListenedMessage.Id == messageId);
                return dispenser != null;
            }
            else
                return false;
        }
        public bool TryGetGuildRoleDispensers(ulong guildId, out IEnumerable<RoleDispenser> dispensers)
        {
            dispensers = default;
            if (_guildDispensers.TryGetValue(guildId, out var dispenserList))
            {
                dispensers = dispenserList.AsEnumerable();
                return dispensers != null;
            }
            else
                return false;
        }
    }
}
