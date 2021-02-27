using System.ComponentModel.DataAnnotations;

namespace NetCoreDiscordBot.Models.Dispensers.References
{
    public class EmoteRolePairReference
    {
        public int Id { get; set; }
        public string Emote { get; set; }
        public ulong RoleId { get; set; }
    }
}
