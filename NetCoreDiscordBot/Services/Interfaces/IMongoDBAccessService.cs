using MongoDB.Driver;

namespace NetCoreDiscordBot.Services.Interfaces
{
    public interface IMongoDBAccessService
    {
        IMongoCollection<T> GetCollection<T>(string collectionName);
        void LaunchMongoDBProcess(string dbPath);
        void KillMongoDBProcess();
    }
}
