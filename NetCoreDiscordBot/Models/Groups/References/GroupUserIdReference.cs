using Microsoft.EntityFrameworkCore;

namespace NetCoreDiscordBot.Models.Groups.References
{
    [Owned]
    public class GroupUserIdReference
    {
        public int Id { get; set; }
        public ulong Value { get; set; }
    }
}
