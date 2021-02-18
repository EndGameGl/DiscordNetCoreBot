using Microsoft.Extensions.Configuration;
using NetCoreDiscordBot.Services.Interfaces;
using System.IO;

namespace NetCoreDiscordBot.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public IConfiguration Configuration { get; }
        public ConfigurationService()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configBuilder.AddJsonFile("appsettings.json");
            Configuration = configBuilder.Build();
        }
    }
}
