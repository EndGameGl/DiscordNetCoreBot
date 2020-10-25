using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace NetCoreDiscordBot.Models.Dispensers.References
{
    [Owned]
    public class EmoteRolePairReference
    {
        public int Id { get; set; }
        public string Emote { get; set; }
        public ulong RoleId { get; set; }
    }
}
