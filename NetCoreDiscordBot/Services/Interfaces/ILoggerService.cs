using System.Threading.Tasks;

namespace NetCoreDiscordBot.Services.Interfaces
{
    public interface ILoggerService
    {
        Task Log(object data);   
    }
}
