using BungieNetCoreAPI;
using BungieNetCoreAPI.Clients;
using BungieNetCoreAPI.Destiny;
using NetCoreDiscordBot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreDiscordBot.Services
{
    public class BungieService : IBungieService
    {
        private readonly BungieClient _client;
        public BungieClient Client => _client;
        public BungieService(IServiceProvider services)
        {
            _client = new BungieClient(
                new BungieClientSettings() 
                { 
                     UseExistingConfig = true,
                     ExistingConfigPath = "BungieConfig.json"
                });
            //Client = new BungieClient(
            //    apiKey: Core.Configuration.GetSection("BungieAPI:ClientKey").Value,
            //    settings: new BungieClientSettings() 
            //    {
            //        UseGlobalCache = true,
            //        PathToLocalDb = @"H:\BungieNetCoreAPIRepository\ManifestDB_14.12.2020",
            //        Locales = new string[] { "en", "ru" }
            //    });
        }       
    }
}
