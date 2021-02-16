using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NetCoreDiscordBot.CustomServices.DecoderPuzzle.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreDiscordBot.CustomServices.DecoderPuzzle.Services
{
    public class LockPuzzleService
    {
        private readonly DiscordSocketClient _discordClient;
        public List<PuzzleHost> Puzzles = new List<PuzzleHost>();

        public LockPuzzleService(IServiceProvider services)
        {
            _discordClient = services.GetRequiredService<DiscordSocketClient>();
        }
        public void Init() { }

    }
}
