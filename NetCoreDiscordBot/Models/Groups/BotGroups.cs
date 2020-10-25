using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreDiscordBot.Models.Groups
{
    public class BotGroups
    {
        public List<Group> Groups { get; set; }
        public BotGroups()
        {
            Groups = new List<Group>();
        }
    }
}
