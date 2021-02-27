using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreDiscordBot.CustomServices.RPG.Services
{
    public class PlayerDataService
    {
        private readonly IMongoCollection<Player> _playersData;
        public PlayerDataService(IServiceProvider services)
        {

        }
    }
}
