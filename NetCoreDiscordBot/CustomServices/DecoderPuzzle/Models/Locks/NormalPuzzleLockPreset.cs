using System.Collections.Generic;

namespace NetCoreDiscordBot.CustomServices.DecoderPuzzle.Models.Locks
{
    public class NormalPuzzleLockPreset : PuzzleLockBase
    {
        private static readonly Dictionary<PuzzleColors, string> ColorsMappings = new Dictionary<PuzzleColors, string>()
        {
            { PuzzleColors.Black, "[0]|[0]|[0]" },
            { PuzzleColors.Blue, "[0]|[0]|[255]" },
            { PuzzleColors.Green, "[0]|[128]|[0]" },
            { PuzzleColors.Red, "[255]|[0]|[0]" },
            { PuzzleColors.White, "[255]|[255]|[255]" }
        };
        public int CaesarShift { get; }
        public NormalPuzzleLockPreset(int lockNumber, PuzzleColors color, int caesarShift) : base(lockNumber, color)
        {
            CaesarShift = caesarShift;
        }
        public override string GetCodedColor()
        {
            return ColorsMappings[Color];
        }
        public override string GetCodedNumber()
        {
            return $"{(LockNumber + CaesarShift).ToString()}:SHIFTED[{(int)(((float)CaesarShift)/10)}:{CaesarShift%10}]";
        }
    }
}
