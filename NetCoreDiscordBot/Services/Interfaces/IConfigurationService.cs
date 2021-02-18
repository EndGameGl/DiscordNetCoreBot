using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreDiscordBot.Services.Interfaces
{
    public interface IConfigurationService
    {
        IConfiguration Configuration { get; }
    }
}
