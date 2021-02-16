using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using NetCoreDiscordBot.CustomServices.DecoderPuzzle.Models.Locks;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.CustomServices.DecoderPuzzle.Models
{
    public class PuzzleHost
    {
        public Guid Guid { get; set; }
        public SocketGuild Guild { get; set; }
        public SocketTextChannel Channel { get; set; }
        public RestUserMessage PresentationMessage { get; set; }
        public List<PuzzleLockBase> Locks { get; set; }
        public PuzzleDifficulty Difficulty { get; set; }
        public int? PreviousLockNumber { get; set; }
        public int? CurrentLockNumber { get; set; }
        public int? NextLockNumber { get; set; }
        public int CurrentOrderIndex { get; set; }
        public List<int> NumbersOrder { get; set; }
        public bool IsFinished { get; private set; }
        public string MessageText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("```");
                sb.AppendLine($"GENERATE::HASH():{RandomValuesGenerator.GetString(50)}");
                sb.AppendLine($"VAULT::LOCK... ({((float)Locks.Count(x => x.IsSolved == false) / 10) * 100}% LOCKED)");
                sb.AppendLine("DECRYPT_ROUTINE::RUN(num, color)");
                PuzzleLockBase prevLockVal = null;
                if (PreviousLockNumber.HasValue) 
                {
                    prevLockVal = Locks.FirstOrDefault(x => x.LockNumber == PreviousLockNumber.Value);
                }
                sb.AppendLine($"LOCKS::DISPLAY_LAST(): {(prevLockVal != null ? $"--|{prevLockVal.GetCodedNumber()}|{prevLockVal.GetCodedColor()}|--" : "--|NULL|--")}");
                PuzzleLockBase curLockVal = null;
                if (CurrentLockNumber.HasValue)
                {
                    curLockVal = Locks.FirstOrDefault(x => x.LockNumber == CurrentLockNumber.Value);
                }
                sb.AppendLine($"LOCKS::DISPLAY_NEXT(): {(curLockVal != null ? $"--|{curLockVal.GetCodedNumber()}|{curLockVal.GetCodedColor()}|--" : "--|NULL|--")}");
                sb.AppendLine($"{(curLockVal != null ? $"NUM_MODE::{{{curLockVal.NumberCypherType.ToString().ToUpper()}}}/\\|/\\COLOR_MODE::{{{curLockVal.ColorCypherType.ToString().ToUpper()}}}" : "")}");
                foreach (var lockBase in Locks)
                {
                    sb.AppendLine($"{(lockBase.IsSolved ? $"[NUMBER:[{lockBase.LockNumber}]|COLOR:[{lockBase.Color.ToString()}]]" : $"[[LOCKED]___[LOCKED]]")}");
                }
                sb.AppendLine("```");
                return sb.ToString();
            }
            private set { }
        }

        private event LockSolvedEventArgs LockSolved;
        private delegate Task LockSolvedEventArgs();

        public event PuzzleSolvedEventArgs PuzzleSolved;
        public delegate void PuzzleSolvedEventArgs(Guid puzzleGuid);

        public PuzzleHost(SocketCommandContext context, PuzzleDifficulty difficulty)
        {
            Guid = Guid.NewGuid();
            Guild = context.Guild;
            Channel = (SocketTextChannel)context.Channel;
            Difficulty = difficulty;
            Locks = new List<PuzzleLockBase>();
            NumbersOrder = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            RegenerateNewLocks(difficulty);
            LockSolved += UpdateMessage;         
        }
        public void RegenerateNewLocks(PuzzleDifficulty difficulty)
        {
            NumbersOrder = NumbersOrder.OrderBy(x => RandomValuesGenerator.GetRandomInt()).ToList();
            Locks.Clear();
            for (int i = 0; i < 10; i++)
            {
                switch (difficulty)
                {
                    case PuzzleDifficulty.Easy:
                            Locks.Add(
                                new EasyPuzzleLockPreset(
                                    number: NumbersOrder[i],
                                    color: (PuzzleColors)Enum.GetValues(typeof(PuzzleColors)).GetValue(RandomValuesGenerator.GetInt(0, 4)),
                                    lowerRange: 0,
                                    upperRange: 9)
                                {
                                    NumberCypherType = PuzzleNumberCypherType.ReverseInRange,
                                    ColorCypherType = PuzzleColorCypherType.TextShuffle
                                });
                        break;
                    case PuzzleDifficulty.Normal:
                        Locks.Add(
                                new NormalPuzzleLockPreset(
                                    lockNumber: NumbersOrder[i],
                                    color: (PuzzleColors)Enum.GetValues(typeof(PuzzleColors)).GetValue(RandomValuesGenerator.GetInt(0, 4)),
                                    caesarShift: RandomValuesGenerator.GetInt(1, 100))
                                {
                                    NumberCypherType = PuzzleNumberCypherType.Caesar,
                                    ColorCypherType = PuzzleColorCypherType.RGB
                                });
                        break;
                    case PuzzleDifficulty.Hard:
                        Locks.Add(
                                new HardPuzzleLockPreset(
                                    lockNumber: NumbersOrder[i],
                                    color: (PuzzleColors)Enum.GetValues(typeof(PuzzleColors)).GetValue(RandomValuesGenerator.GetInt(0, 4)))
                                {
                                    NumberCypherType = PuzzleNumberCypherType.Binary,
                                    ColorCypherType = PuzzleColorCypherType.Hex
                                });
                        break;
                }
            }

            PreviousLockNumber = null;
            CurrentLockNumber = null;
            NextLockNumber = null;
            CurrentOrderIndex = 0;
            CurrentLockNumber = NumbersOrder[CurrentOrderIndex];
            NextLockNumber = NumbersOrder[CurrentOrderIndex + 1];
        }
        public async Task SendMessage()
        {
            PresentationMessage = await Channel.SendMessageAsync(MessageText);
        }
        public async Task UpdateMessage()
        {
            await PresentationMessage.ModifyAsync(x => x.Content = MessageText);
        }
        public async Task TrySolve(string number, string input, SocketGuildUser solver)
        {
            if (IsFinished != true)
            {
                if (CurrentLockNumber.HasValue)
                {
                    var curLock = Locks.FirstOrDefault(x => x.LockNumber == CurrentLockNumber.Value);
                    if (curLock.TrySolve(number, input, solver))
                    {                      
                        CurrentOrderIndex++;
                        if (CurrentOrderIndex >= 10)
                        {
                            IsFinished = true;
                            StringBuilder builder = new StringBuilder();
                            builder.AppendLine("```LOCK::LIFTED");
                            builder.AppendLine("SOLVERS::PRINT()");
                            var users = Locks.Select(x => x.Solver).Distinct();
                            foreach (var user in users)
                            {
                                int amountSolved = Locks.Count(x => x.Solver.Id == user.Id);
                                builder.AppendLine($"SOLVER::{user.Nickname}|ID::{user.Id}|SOLVED::{amountSolved}");
                            }
                            builder.AppendLine($"|{new string('-', 10)}PUZZLE_OVER{new string('-', 10)}|```");
                            await PresentationMessage.ModifyAsync(x => x.Content = builder.ToString());
                            PuzzleSolved?.Invoke(this.Guid);
                        }
                        else
                        {
                            if (CurrentOrderIndex > 0)
                                PreviousLockNumber = NumbersOrder[CurrentOrderIndex - 1];
                            else
                                PreviousLockNumber = null;
                            if (NumbersOrder.ElementAtOrDefault(CurrentOrderIndex) != 10)
                                CurrentLockNumber = NumbersOrder[CurrentOrderIndex];
                            else
                                CurrentLockNumber = null;
                            if (NumbersOrder.ElementAtOrDefault(CurrentOrderIndex + 1) != 0)
                                NextLockNumber = NumbersOrder[CurrentOrderIndex + 1];
                            else
                                NextLockNumber = null;
                            await LockSolved?.Invoke();
                        }
                    }
                }
            }
        }
    }
}
