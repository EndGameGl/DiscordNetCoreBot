using Discord.WebSocket;

namespace NetCoreDiscordBot.CustomServices.DecoderPuzzle.Models.Locks
{
    public abstract class PuzzleLockBase
    {
        public SocketGuildUser Solver { get; set; }
        public int LockNumber { get; }
        public PuzzleColors Color { get; }
        public PuzzleColorCypherType ColorCypherType { get; set; }
        public PuzzleNumberCypherType NumberCypherType { get; set; }
        protected PuzzleLockBase(int lockNumber, PuzzleColors color)
        {
            LockNumber = lockNumber;
            Color = color;
        }

        public bool IsSolved { get; protected set; }
        public abstract string GetCodedNumber();
        public abstract string GetCodedColor();
        public bool TrySolve(string number, string color, SocketGuildUser solver)
        {
            bool isValidNumber = false;
            bool isValidColor = false;
            if (int.TryParse(number, out var num))
            {
                isValidNumber = num == LockNumber;
            }
            else
                isValidNumber = false;
            isValidColor = Color.ToString().ToUpper() == color.ToUpper();

            if (IsSolved != true)
            {
                IsSolved = (isValidNumber == true) && (isValidColor == true);
            }
            if (IsSolved)
                Solver = solver;
            return IsSolved;
        }
    }
}
