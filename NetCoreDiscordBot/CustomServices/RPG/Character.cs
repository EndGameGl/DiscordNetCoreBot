using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreDiscordBot.CustomServices.RPG
{
    public abstract class Character
    {
        public int Level { get; set; }

        public bool IsFighting { get; set; }

        public double MaxHP { get; set; }
        public double CurrentHP { get; set; }

        public double MaxMP { get; set; }
        public double CurrentMP { get; set; }

        public double ATK { get; set; }
        public double Defense { get; set; }
        public double Evasion { get; set; }
        public double Accuracy { get; set; }
    }
}
