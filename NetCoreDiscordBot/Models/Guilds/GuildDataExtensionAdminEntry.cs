using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreDiscordBot.Models.Guilds
{
    [Owned]
    public class GuildDataExtensionAdminEntry
    {
        public int Id { get; set; }
        public ulong AdminId { get; set; }
        public GuildDataExtensionAdminEntry() { }
    }
}
