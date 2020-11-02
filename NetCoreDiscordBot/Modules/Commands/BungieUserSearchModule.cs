using Discord.Commands;
using NetCoreDiscordBot.Services;
using System.Threading.Tasks;
using BungieAPI.User;

namespace NetCoreDiscordBot.Modules.Commands
{
    [Group("bungie")]
    public class BungieUserSearchModule : ModuleBase<SocketCommandContext>
    {
        private BungieAPIService _bungieService;
        public BungieUserSearchModule(BungieAPIService service)
        {
            _bungieService = service;
        }

        [Command("search user")]
        public async Task SearchUser(string userName)
        { 
            var result = await _bungieService.Client.SearchUsers(userName);
            if (result.Response != null)
            {
                var foundUsers = result.Response;
                string message = "Search results:\n";
                int i = 1;
                foreach (var user in foundUsers)
                {
                    message += $"{i}) Name: {user.DisplayName}, Id: {user.MembershipID}\n";
                    i++;
                }
                await ReplyAsync(message);
            }
            else
                await ReplyAsync("No users found.");
        }
    }
}
