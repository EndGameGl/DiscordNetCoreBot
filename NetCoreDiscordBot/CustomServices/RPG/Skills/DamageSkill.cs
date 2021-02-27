using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreDiscordBot.CustomServices.RPG.Skills
{
    public class DamageSkill : ISkill
    {
        public double DamageDealt { get; set; }
        public double CharacterATKScaleFactor { get; set; }
        public string Name { get; set; }
        public double MPCost { get; set; }

        public bool CanUse(Character usedBy)
        {
            return usedBy.CurrentMP > MPCost;
        }

        public SkillUseResult UseOn(Character usedBy, Character usedOn)
        {
            SkillUseResult result = new SkillUseResult();
            var presumedDMG = usedBy.ATK * CharacterATKScaleFactor + DamageDealt;
            var resultingDMG = presumedDMG - usedOn.Defense;

            double successChance = 1;
            if (usedOn.Evasion > usedBy.Accuracy)
                successChance -= 0.35;
            if (usedOn.Level > usedBy.Level)
                successChance -= 0.35;

            bool didSucceed = RandomValuesGenerator.GetBool(successChance);

            if (didSucceed)
            {
                result.DidDamage = true;
                result.Amount = resultingDMG;
                result.DidSuccess = true;
            }
            else
            {
                result.DidDamage = false;
                result.Amount = 0;
                result.DidSuccess = false;
            }

            return result;
        }
    }
}
