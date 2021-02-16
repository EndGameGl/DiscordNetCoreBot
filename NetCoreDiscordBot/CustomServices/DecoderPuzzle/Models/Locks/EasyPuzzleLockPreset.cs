using System;

namespace NetCoreDiscordBot.CustomServices.DecoderPuzzle.Models.Locks
{
    public class EasyPuzzleLockPreset : PuzzleLockBase
    {
        private int _lowerRange;
        private int _upperRange;
        public EasyPuzzleLockPreset(int number, PuzzleColors color, int lowerRange, int upperRange) : base(number, color)
        {
            _lowerRange = lowerRange;
            _upperRange = upperRange;
        }

        public override string GetCodedColor()
        {
            switch (Color)
            {
                case PuzzleColors.Black:
                    return Shuffle("Black");
                case PuzzleColors.Blue:
                    return Shuffle("Blue");
                case PuzzleColors.Green:
                    return Shuffle("Green");
                case PuzzleColors.Red:
                    return Shuffle("Red");
                case PuzzleColors.White:
                    return Shuffle("White");
                default:
                    throw new Exception();
            }
        }
        public override string GetCodedNumber()
        {
            return ((_lowerRange + _upperRange) - LockNumber).ToString();
        }
        private string Shuffle(string str)
        {
            char[] array = str.ToCharArray();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = RandomValuesGenerator.GetInt(0, n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return new string(array);
        }
    }
}
