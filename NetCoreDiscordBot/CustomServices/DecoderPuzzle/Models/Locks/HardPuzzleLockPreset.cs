using System;
using System.Collections.Generic;

namespace NetCoreDiscordBot.CustomServices.DecoderPuzzle.Models.Locks
{
    public class HardPuzzleLockPreset : PuzzleLockBase
    {
        private static readonly Dictionary<PuzzleColors, string> ColorsMappings = new Dictionary<PuzzleColors, string>()
        {
            { PuzzleColors.Black, "#000000" },
            { PuzzleColors.Blue, "#0000FF" },
            { PuzzleColors.Green, "#008000" },
            { PuzzleColors.Red, "#FF0000" },
            { PuzzleColors.White, "#FFFFFF" }
        };
        public HardPuzzleLockPreset(int lockNumber, PuzzleColors color) : base(lockNumber, color) { }
        public override string GetCodedColor()
        {
            return ColorsMappings[Color];
        }
        public override string GetCodedNumber()
        {
            return IntToBinaryString(LockNumber);
        }
        private string IntToBinaryString(int value)
        {
            string binVal = Convert.ToString(value, 2);
            int bits = 0;
            int bitblock = 4;

            for (int i = 0; i < binVal.Length; i += bitblock)
            { 
                bits += bitblock;
            }

            return binVal.PadLeft(bits, '0');
        }
    }
}
