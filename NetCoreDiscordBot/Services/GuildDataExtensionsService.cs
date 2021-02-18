using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NetCoreDiscordBot.Models.Guilds;
using NetCoreDiscordBot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services
{
    public class GuildDataExtensionsService : IGuildDataExtensionsService
    {
        private readonly DiscordSocketClient _discordClient;
        private Dictionary<ulong, GuildDataExtension> _guildsData { get; set; }

        private readonly IMongoCollection<GuildDataExtension> _guildDataDB;
        public GuildDataExtensionsService(IServiceProvider services)
        {
            _discordClient = services.GetRequiredService<DiscordSocketClient>();
            _guildsData = new Dictionary<ulong, GuildDataExtension>();

            var config = services.GetRequiredService<IConfigurationService>();
            var collectionName = config.Configuration.GetSection("MongoDB:GuildSettingsDatabase").Value;
            _guildDataDB = services.GetRequiredService<IMongoDBAccessService>().GetCollection<GuildDataExtension>(collectionName);

            _discordClient.JoinedGuild += JoinedGuildHandler;
            _discordClient.LeftGuild += LeftGuildHandler;
        }
        public async Task InitializeAsync()
        {
            foreach (var guild in _discordClient.Guilds)
            {
                if (!_guildDataDB.Find(x => x.GuildId == guild.Id).Any())
                {
                    var extension = new GuildDataExtension() { GuildId = guild.Id };
                    _guildsData.Add(guild.Id, extension);
                    await _guildDataDB.InsertOneAsync(extension);
                }
                else
                {
                    _guildsData.Add(guild.Id, await _guildDataDB.Find(x => x.GuildId == guild.Id).FirstOrDefaultAsync());
                }
            }
        }
        private async Task JoinedGuildHandler(SocketGuild guild)
        {
            var extensionData = await _guildDataDB.Find(x => x.GuildId == guild.Id).FirstOrDefaultAsync();
            if (extensionData != null)
                _guildsData.Add(guild.Id, extensionData);
            else
                _guildsData.Add(guild.Id, new GuildDataExtension() { GuildId = guild.Id });
            await SaveGuildData(guild.Id);
        }
        private async Task LeftGuildHandler(SocketGuild guild)
        {
            _guildsData.Remove(guild.Id);
        }
        public bool IsAdmin(ulong guildId, ulong userId)
        {
            if (TryGetData(guildId, out var data))
            {
                return data.Admins.Any(x => x == userId);
            }
            else
                return false;
        }
        public bool TryGetData(ulong guildId, out GuildDataExtension data)
        {
            return _guildsData.TryGetValue(guildId, out data);
        }
        public async Task SaveGuildData(ulong id)
        {
            await _guildDataDB.ReplaceOneAsync(x => x.GuildId == id, _guildsData[id]);
        }
    }
}
