using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NetCoreDiscordBot.Models.Guilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services
{
    public class GuildDataExtensionsService
    {
        private readonly DiscordShardedClient _discordClient;
        private readonly GuildDataExtensionsContext _dataBase;
        public Dictionary<SocketGuild, GuildDataExtension> ExtendedGuilds { get; set; }
        public GuildDataExtensionsService(IServiceProvider services)
        {
            _discordClient = services.GetRequiredService<DiscordShardedClient>();
            _dataBase = services.GetRequiredService<GuildDataExtensionsContext>();
            ExtendedGuilds = new Dictionary<SocketGuild, GuildDataExtension>();

            _discordClient.JoinedGuild += JoinedGuildHandler;
            _discordClient.LeftGuild += LeftGuildHandler;
        }
        public async Task InitializeAsync()
        {
            foreach (var guild in _discordClient.Guilds)
            {
                if (!_dataBase.GuildDataExtensions.Any(x => x.GuildId == guild.Id))
                {
                    var extension = new GuildDataExtension() { GuildId = guild.Id };
                    ExtendedGuilds.Add(guild, extension);
                    _dataBase.GuildDataExtensions.Add(extension);
                }
                else
                {
                    ExtendedGuilds.Add(guild, _dataBase.GuildDataExtensions.FirstOrDefault(x => x.GuildId == guild.Id));
                }
            }
            await _dataBase.SaveChangesAsync();
        }
        private async Task JoinedGuildHandler(SocketGuild guild)
        {
            var extensionData = _dataBase.GuildDataExtensions.FirstOrDefault(x => x.GuildId == guild.Id);
            if (extensionData != null)
                ExtendedGuilds.Add(guild, extensionData);
            else
                ExtendedGuilds.Add(guild, new GuildDataExtension() { GuildId = guild.Id });
            await _dataBase.SaveChangesAsync();
        }
        private async Task LeftGuildHandler(SocketGuild guild)
        {
            ExtendedGuilds.Remove(guild);
        }
        public async Task ForceSaveDb()
        {
            await _dataBase.SaveChangesAsync();
        }
        public bool CheckIfAdmin(ulong guildId, ulong userId)
        {
            var extendedGuild = ExtendedGuilds.FirstOrDefault(x => x.Key.Id == guildId);
            return extendedGuild.Value.Admins.Any(x => x.AdminId == userId);
        }
    }
}
