using Discord.Commands;
using Discord.WebSocket;
using NetCoreDiscordBot.CustomServices.DecoderPuzzle.Models;
using NetCoreDiscordBot.CustomServices.DecoderPuzzle.Models.Locks;
using NetCoreDiscordBot.CustomServices.DecoderPuzzle.Services;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.CustomServices.DecoderPuzzle.Modules
{
    [Group("puzzle")]
    public class PuzzleCommandModule : ModuleBase<SocketCommandContext>
    {
        private LockPuzzleService _puzzleService;
        public PuzzleCommandModule(LockPuzzleService puzzleService) 
        {
            _puzzleService = puzzleService;
        }

        [Command("create"), RequireOwner]
        public async Task CreatePuzzle(string difficulty)
        {
            PuzzleDifficulty? diff = null;
            switch (difficulty.ToUpper())
            {
                case "EASY":
                    diff = PuzzleDifficulty.Easy;
                    break;
                case "NORMAL":
                    diff = PuzzleDifficulty.Normal;
                    break;
                case "HARD":
                    diff = PuzzleDifficulty.Hard;
                    break;
            }

            if (!diff.HasValue)
            {
                await ReplyAsync("Wrong difficulty");
                return;
            }
            
            PuzzleHost puzzle = new PuzzleHost(Context, diff.Value);
            puzzle.RegenerateNewLocks(diff.Value);
            await puzzle.SendMessage();
            puzzle.PuzzleSolved += (guid) =>
            {
                var puzzleInService = _puzzleService.Puzzles.FirstOrDefault(x => x.Guid == guid);
                _puzzleService.Puzzles.Remove(puzzleInService);
            };
            _puzzleService.Puzzles.Add(puzzle);
        }

        [Command("solve")]
        public async Task TrySolvePuzzle(string number, params string[] color)
        {
            var puzzle = _puzzleService.Puzzles.FirstOrDefault(x => x.Guild.Id == Context.Guild.Id);
            if (puzzle != null)
                await puzzle.TrySolve(number, string.Join("", color), (SocketGuildUser)Context.User);
        }

        [Command("as json"), RequireOwner]
        public async Task AsJSON(ulong messageId)
        {
            var puzzle = _puzzleService.Puzzles.FirstOrDefault(x => x.PresentationMessage.Id == messageId);
            var reference = new
            {
                Guid = puzzle.Guid.ToString(),
                ServerId = puzzle.Guild.Id,
                ChannelId = puzzle.Channel.Id,
                MessageId = puzzle.PresentationMessage.Id,
                Difficulty = puzzle.Difficulty.ToString(),
                Locks = puzzle.Locks.Select(x => new { Number = x.LockNumber, Color = x.Color }),
                Order = puzzle.NumbersOrder
            };
            await ReplyAsync($"```{JsonConvert.SerializeObject(reference, Formatting.Indented)}```");
        }
    }
}
