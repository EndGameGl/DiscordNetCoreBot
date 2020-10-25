using BungieAPI;
using BungieAPI.Client;

namespace NetCoreDiscordBot.Services
{
    public class BungieAPIService
    {
        public BungieClient Client { get; set; }
        public BungieAPIService()
        {
            Client = new BungieClient(
                key: Core.Configuration["BungieAPI:ClientKey"],
                clientID: int.Parse(Core.Configuration["BungieAPI:ClientID"]),
                clientSecret: Core.Configuration["BungieAPI:ClientSecret"],
                new BungieClientSettings() { PreloadManifest = false });
        }
    }
}
