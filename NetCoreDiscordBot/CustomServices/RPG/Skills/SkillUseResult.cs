using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreDiscordBot.CustomServices.RPG.Skills
{
    public class SkillUseResult
    {
        public bool DidSuccess { get; set; }
        public bool DidDamage { get; set; }
        public double Amount { get; set; }
    }
}
