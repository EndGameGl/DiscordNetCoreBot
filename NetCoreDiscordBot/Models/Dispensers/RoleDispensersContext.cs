using Microsoft.EntityFrameworkCore;
using NetCoreDiscordBot.Models.Dispensers.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Models.Dispensers
{
    public class RoleDispensersContext : DbContext
    {
        public DbSet<RoleDispenserReference> DispenseresReferences { get; set; }

        public RoleDispensersContext(DbContextOptions<RoleDispensersContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        public async Task<List<RoleDispenserReference>> GetDispenserReferences(ulong guildId)
        {
            Console.WriteLine($"Loading dispensers for {guildId}");
            return await DispenseresReferences.AsAsyncEnumerable().Where(x => x.GuildId == guildId).ToListAsync();
        }
    }
}
