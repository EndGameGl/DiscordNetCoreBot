using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace NetCoreDiscordBot.Models.Groups
{
    public class GroupUserList
    {
        public string Description { get; set; }
        public IEmote JoinEmote { get; set; }
        public List<SocketGuildUser> Users { get; set; }
        public int? UserLimit { get; set; }
        public bool HasPlaceForNewUsers 
        { 
            get 
            {
                if (!UserLimit.HasValue)
                    return true;
                else if (Users.Count < UserLimit.Value)
                    return true;
                else
                    return false;
            } 
            private set { } 
        }
        public GroupUserList(string description, IEmote joinEmote, int? userLimit = null)
        {
            Description = description;
            JoinEmote = joinEmote;
            Users = new List<SocketGuildUser>();
            if (userLimit.HasValue)
                UserLimit = userLimit.Value;
            else
                UserLimit = null;
        }
        public GroupUserList() { }

        public bool TryJoinList(SocketGuildUser newUser)
        {
            if (HasPlaceForNewUsers)
            {
                if (!Users.Any(x => x.Id == newUser.Id))
                {
                    Users.Add(newUser);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
    }
}
