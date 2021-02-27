using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreDiscordBot.Models
{
    public class UserData
    {
        [BsonId]
        public ulong Id { get; set;  }
        public Dictionary<string, string> Cache { get; set; }
        public long MessagesSent { get; set; }
        public long ReactionsAdded { get; set; }
        public long GroupsCreated { get; set; }
    }
}
