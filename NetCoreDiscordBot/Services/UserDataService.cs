using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NetCoreDiscordBot.Models;
using NetCoreDiscordBot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services
{
    public class UserDataService : IUserDataService
    {
        //private Timer _updateTimer;
        private readonly ILoggerService _logger;
        private readonly DiscordSocketClient _discordClient;

        private readonly IMongoCollection<UserData> _userData;

        public UserDataService(IServiceProvider services)
        {
            var config = services.GetRequiredService<IConfigurationService>();
            var collectionName = config.Configuration.GetSection("MongoDB:UserDataDatabase").Value;

            _logger = services.GetRequiredService<ILoggerService>();
            _discordClient = services.GetRequiredService<DiscordSocketClient>();

            _userData = services.GetRequiredService<IMongoDBAccessService>().GetCollection<UserData>(collectionName);

            //_updateTimer = new Timer();
        }

        public async Task<UserData> GetDataAsync(SocketUser user)
        {
            UserData data = default;
            var queryResult = _userData.Find(x => x.Id == user.Id);
            var exists = await queryResult.AnyAsync();
            if (exists == false)
            {
                data = new UserData() { Id = user.Id, Cache = new Dictionary<string, string>() };
                await _userData.InsertOneAsync(data);
            }
            else
                data = await queryResult.FirstOrDefaultAsync();
            return data;
        }
        public async Task SaveAsync(UserData data)
        {
            await _userData.ReplaceOneAsync(x => x.Id == data.Id, data);
        }
    }
}
