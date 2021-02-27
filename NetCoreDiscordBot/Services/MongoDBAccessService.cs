using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NetCoreDiscordBot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NetCoreDiscordBot.Services
{
    public class MongoDBAccessService : IMongoDBAccessService
    {
        private Process _mongoDBProcess;

        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _botDB;

        public MongoDBAccessService(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetRequiredService<IConfigurationService>();

            var connectionString = config.Configuration.GetSection("MongoDB:ConnectionString").Value;
            var dbName = config.Configuration.GetSection("MongoDB:DatabaseName").Value;
            var dbPath = $"{Environment.CurrentDirectory}\\{config.Configuration.GetSection("MongoDB:DBPath").Value}";

            if (!Directory.Exists(dbPath))
                throw new Exception($"Directory doesn't exist: {dbPath}");

            LaunchMongoDBProcess(dbPath);

            _mongoClient = new MongoClient($"{connectionString}");
            _botDB = _mongoClient.GetDatabase(dbName);

            ValidateCollections(config);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _botDB.GetCollection<T>(collectionName);
        }

        public void LaunchMongoDBProcess(string dbPath)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = $"{dbPath}\\mongod.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"--dbpath {dbPath}",               
            };

            _mongoDBProcess = Process.Start(startInfo);
        }
        public void KillMongoDBProcess()
        {
            _mongoDBProcess?.Kill();
        }
        private void ValidateCollections(IConfigurationService configs)
        {
            var groupsDB = configs.Configuration.GetSection("MongoDB:GroupsDatabase").Value;
            var dispensersDB = configs.Configuration.GetSection("MongoDB:RoleDispensersDatabase").Value;
            var guildSettingsDB = configs.Configuration.GetSection("MongoDB:GuildSettingsDatabase").Value;
            var userDataDB = configs.Configuration.GetSection("MongoDB:UserDataDatabase").Value;

            var existingCollections = _botDB.ListCollectionNames().ToList();

            if (!existingCollections.Contains(groupsDB))
                _botDB.CreateCollection(groupsDB);

            if (!existingCollections.Contains(dispensersDB))
                _botDB.CreateCollection(dispensersDB);

            if (!existingCollections.Contains(guildSettingsDB))
                _botDB.CreateCollection(guildSettingsDB);

            if (!existingCollections.Contains(userDataDB))
                _botDB.CreateCollection(userDataDB);
        }
    }
}
