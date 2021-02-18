using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetCoreDiscordBot.Models.Dispensers.References
{
    public class RoleDispenserReference
    {
        [BsonId]
        public int _id;
        public Guid Guid { get; set; }
        public string Description { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong? MessageId { get; set; }
        public List<EmoteRolePairReference> Bindings { get; set; }

        public RoleDispenserReference() { }

        public RoleDispenserReference(RoleDispenser dispenser)
        {
            Guid = dispenser.Guid;
            Description = dispenser.Description;
            GuildId = dispenser.Guild.Id;
            ChannelId = dispenser.Channel.Id;
            MessageId = dispenser.ListenedMessage?.Id;
            Bindings = new List<EmoteRolePairReference>();
            foreach (var key in dispenser.EmoteToRoleBindings.Keys)
            {
                Bindings.Add(
                    new EmoteRolePairReference() 
                    { 
                        Emote = key.Name, 
                        RoleId = dispenser.EmoteToRoleBindings[key].Id 
                    });
            }
        }
        public void Update(RoleDispenser dispenser)
        {
            Guid = dispenser.Guid;
            Description = dispenser.Description;
            GuildId = dispenser.Guild.Id;
            ChannelId = dispenser.Channel.Id;
            MessageId = dispenser.ListenedMessage.Id;
            Bindings = new List<EmoteRolePairReference>();
            foreach (var key in dispenser.EmoteToRoleBindings.Keys)
            {
                Bindings.Add(
                    new EmoteRolePairReference()
                    {
                        Emote = key.Name,
                        RoleId = dispenser.EmoteToRoleBindings[key].Id
                    });
            }
        }
    }
}
