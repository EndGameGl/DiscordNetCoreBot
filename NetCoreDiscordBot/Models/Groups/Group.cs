using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Models.Groups
{
    public class Group
    {       
        public Guid Guid { get; set; }
        public SocketGuildUser Host { get; set; }
        public SocketGuild Guild { get; set; }
        public SocketTextChannel Channel { get; set; }
        public List<GroupUserList> UserLists { get; set; }
        public RestUserMessage PresentationMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public GroupType Type { get; set; }

        public bool IsFull => UserLists.TrueForAll(x => !x.HasPlaceForNewUsers);
        public Group(SocketGuild guild, SocketGuildUser host, SocketTextChannel channel, List<GroupUserList> userLists, string description, GroupType type)
        {
            Guid = Guid.NewGuid();
            Guild = guild;
            Host = host;
            Channel = channel;
            UserLists = userLists;
            CreatedAt = DateTime.Now;
            Description = description;
            Type = type;
        }
        public Group() { }

        private string GetMessageText()
        {
            StringBuilder messageBuilder = new StringBuilder();
            switch (Type)
            {
                case GroupType.Simple:
                    messageBuilder.Append(
                        $"Собирается группа пользователем {Host.Mention}: {Description}\n" +
                        $"__**Осталось {UserLists[0].UserLimit - UserLists[0].Users.Count()} мест**__\n");
                    break;
                case GroupType.Poll:
                    messageBuilder.Append($"Пользователь {Host.Mention} предлагает проголосовать:\n");
                    break;
            }
            foreach (var list in UserLists)
            {
                messageBuilder.Append($"{list.Description} ({list.JoinEmote.Name}):\n");
                foreach (var user in list.Users)
                {
                    messageBuilder.Append($"> {user.Mention}\n");
                }
            }
            return messageBuilder.ToString();
        }
        public async Task SendMessage(string callEmoji, string closeEmoji)
        {
            PresentationMessage = await Channel.SendMessageAsync(GetMessageText());
            var emojis = UserLists.Select(x => x.JoinEmote).ToList();
            emojis.Add(new Emoji(callEmoji));
            emojis.Add(new Emoji(closeEmoji));
            await PresentationMessage.AddReactionsAsync(emojis.ToArray());
        }
        public async Task UpdateMessage()
        {
            await PresentationMessage.ModifyAsync(x => x.Content = GetMessageText());
        }
        public async Task CloseMessage()
        {
            switch (Type)
            {
                case GroupType.Simple:
                    await PresentationMessage.ModifyAsync(m =>
                    {
                        m.Content = $"Сбор группы {Host.Mention} ({Description}) завершен.";
                    });
                    break;
                case GroupType.Poll:
                    break;
            }
            await PresentationMessage.RemoveAllReactionsAsync();
        }
        public async Task SendAnnouncement(SocketUser announceBy)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (Type == GroupType.Simple)
            {
                stringBuilder.AppendLine($"{announceBy.Mention} объявляет сбор группы: {Description}");
            }
            else
            {
                stringBuilder.AppendLine($"Голосование завершено: {Description}");
            }
            foreach (var list in UserLists)
            {
                stringBuilder.AppendLine($"{list.Description}:");
                foreach (var user in list.Users)
                {
                    stringBuilder.AppendLine($"> {user.Mention}");
                }
            }
            await Channel.SendMessageAsync(stringBuilder.ToString());
        }
        public async Task ReloadGroup()
        {
            foreach (var list in UserLists)
            {
                var listCheckTreshold = list.UserLimit ?? 50;
                var reactions = (await PresentationMessage.GetReactionUsersAsync(list.JoinEmote, listCheckTreshold).FlattenAsync()).Cast<SocketGuildUser>();
                list.Users = list.Users.Intersect(reactions).Take(listCheckTreshold).ToList();
            }
            await UpdateMessage();
        }
        public override string ToString()
        {
            return $"Group: {Description}, {UserLists.Sum(x => x.Users.Count)} users";
        }
    }
}
