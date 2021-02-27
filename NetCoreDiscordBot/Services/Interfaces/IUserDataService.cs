using Discord.WebSocket;
using NetCoreDiscordBot.Models;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services.Interfaces
{
    public interface IUserDataService
    {
        Task<UserData> GetDataAsync(SocketUser user);
        Task SaveAsync(UserData data);
    }
}
