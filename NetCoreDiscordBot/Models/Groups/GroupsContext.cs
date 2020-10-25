using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreDiscordBot.Models.Groups.References;
using NetCoreDiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Models.Groups
{
    public class GroupsContext : DbContext
    {
        public DbSet<GroupReference> Groups { get; set; }
        public GroupsContext(DbContextOptions<GroupsContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        public async Task<List<GroupReference>> GetGroupReferences(ulong guildId)
        {
            Console.WriteLine($"Loading groups for {guildId}");
            return await Groups.AsAsyncEnumerable().Where(x => x.GuildId == guildId).ToListAsync();
        }
    }
}
