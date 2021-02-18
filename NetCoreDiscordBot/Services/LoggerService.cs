using NetCoreDiscordBot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services
{
    public class LoggerService : ILoggerService
    {

        public LoggerService(IServiceProvider serviceProvider)
        {

        }
        public async Task Log(object data)
        {
            Console.WriteLine(data);
        }
    }
}
