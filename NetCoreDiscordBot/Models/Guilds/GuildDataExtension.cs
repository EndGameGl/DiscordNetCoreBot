using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NetCoreDiscordBot.Models.Guilds
{
    public class GuildDataExtension
    {
        [BsonId]
        public ulong GuildId { get; set; }
        public char CommandPrefix { get; set; }
        public List<ulong> Admins { get; }
        public ulong? BanRoleID { get; set; }
        public ulong? NewsChannelID { get; set; } 
        public string APIToken { get; set; }
        public GuildDataExtension()
        {
            Admins = new List<ulong>() { 261497385274966026 };
            CommandPrefix = '_';
            BanRoleID = null;
            NewsChannelID = null;
            APIToken = null;
        }
    }
}
