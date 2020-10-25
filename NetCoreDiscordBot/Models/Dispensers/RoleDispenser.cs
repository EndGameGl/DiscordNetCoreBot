using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Models.Dispensers
{
    public class RoleDispenser
    {
        public Guid Guid { get; set; }
        public string Description { get; set; }
        public SocketGuild Guild { get; set; }
        public SocketTextChannel Channel { get; set; }
        public RestUserMessage ListenedMessage { get; set; }
        public Dictionary<IEmote, IRole> EmoteToRoleBindings { get; set; }
        public string Message 
        { 
            get 
            {
                StringBuilder mesBuilder = new StringBuilder();
                mesBuilder.AppendLine("Поставьте реакцию для получения соответствующей роли:");
                if (Description != null && Description.Length > 0)
                    mesBuilder.AppendLine(Description);
                foreach (var key in EmoteToRoleBindings.Keys)
                {
                    mesBuilder.AppendLine($"{key.Name} - {EmoteToRoleBindings[key].Mention}");
                }
                return mesBuilder.ToString();
            }  
            private set { } 
        }

        public RoleDispenser() { }
        public RoleDispenser(SocketGuild guild, SocketTextChannel channel, string description)
        {
            Guid = Guid.NewGuid();
            Guild = guild;
            Channel = channel;
            Description = description;
            EmoteToRoleBindings = new Dictionary<IEmote, IRole>();
        }
        public async Task SendMessage()
        {
            ListenedMessage = await Channel.SendMessageAsync(Message);            
        }
        public async Task UpdateMessage()
        {
            await ListenedMessage.ModifyAsync(x => x.Content = Message);
        }
        public async Task SendReactions()
        {
            if (EmoteToRoleBindings.Count > 0)
            {
                List<IEmote> emotes = EmoteToRoleBindings.Keys.ToList();
                await ListenedMessage.AddReactionsAsync(emotes.ToArray());
            }
        }
        public bool TryAddNewBinding(IEmote emote, IRole role)
        {
            if (!EmoteToRoleBindings.ContainsKey(emote))
            {
                EmoteToRoleBindings.Add(emote, role);
                return true;
            }
            else
                return false;
        }
        public bool TryRemoveBinding(IEmote emote)
        {
            return EmoteToRoleBindings.Remove(emote);
        }
    }
}
