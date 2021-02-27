using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreDiscordBot.CustomServices.RPG.Skills
{
    public interface ISkill
    {
        string Name { get; set; }
        double MPCost { get; set; }
        bool CanUse(Character usedBy);
        SkillUseResult UseOn(Character usedBy, Character usedOn);
    }
}
