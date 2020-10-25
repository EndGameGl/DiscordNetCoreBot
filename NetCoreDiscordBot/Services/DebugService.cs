using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services
{
    public class DebugService
    {
        public void Log(object data)
        {
            Console.WriteLine(data);
        }
    }
}
