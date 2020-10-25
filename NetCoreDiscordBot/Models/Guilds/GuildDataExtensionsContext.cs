using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreDiscordBot.Models.Guilds
{
    public class GuildDataExtensionsContext : DbContext
    {
        public DbSet<GuildDataExtension> GuildDataExtensions { get; set; }
        public GuildDataExtensionsContext(DbContextOptions<GuildDataExtensionsContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}
