using BungieNetCoreAPI.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreDiscordBot.Services.Interfaces
{
    public interface IBungieService
    {
        public BungieClient Client { get; }
    }
}
