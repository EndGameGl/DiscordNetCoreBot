using Discord;
using Discord.Commands;
using NetCoreDiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Modules.Commands
{
    [Group("dsc")]
    public class DestinyDeepStoneCryptModule : ModuleBase<SocketCommandContext>
    {
        private readonly GroupHandlingService _groupService;
        public DestinyDeepStoneCryptModule(GroupHandlingService groupService)
        {
            _groupService = groupService;
        }

        [Command("roles 4")]
        public async Task AssignRolesStageFour(ulong groupMessageId)
        {
            if (_groupService.GuildGroupLists.TryGetValue(Context.Guild.Id, out var groups))
            {
                var group = groups.FirstOrDefault(x => x.PresentationMessage.Id == groupMessageId);
                if (group.UserLists.Count == 1 && group.UserLists.First().UserLimit == 6 && group.IsFull)
                {
                    var users = group.UserLists.First();
                    await ReplyAsync(
                        $"1 ядро. Операторы ({users.Users[0]}, {users.Users[1]}):  1-5\n" +
                        $"2 ядро. Подавители ({users.Users[2]}, {users.Users[3]}): 2-6\n" +
                        $"3 ядро. {users.Users[4]}: 3-5\n" +
                        $"4 ядро. {users.Users[5]}: 4-6");
                }
                else
                    await ReplyAsync("Группа не подходит под условия");
            }
        }
        [Command("roles 3")]
        public async Task AssignRolesStageThree(ulong groupMessageId)
        {
            if (_groupService.GuildGroupLists.TryGetValue(Context.Guild.Id, out var groups))
            {
                var group = groups.FirstOrDefault(x => x.PresentationMessage.Id == groupMessageId);
                if (group.UserLists.Count == 1 && group.UserLists.First().UserLimit == 6 && group.IsFull)
                {
                    var users = group.UserLists.First();

                    var operatorRole = Context.Guild.GetRole(793847034901692438);
                    var scannerRole = Context.Guild.GetRole(793847113985032192);
                    var supressorRole = Context.Guild.GetRole(793847173502074880);

                    EmbedBuilder embedBuilder = new EmbedBuilder();
                    embedBuilder.WithTitle("Склеп Глубокого Камня: Испытание \"На все руки\"");
                    embedBuilder.AddField("1 раунд", $"{operatorRole.Mention}: {users.Users[2].Mention}\n{scannerRole.Mention}: {users.Users[0].Mention}\n{supressorRole.Mention}: {users.Users[4].Mention}");
                    embedBuilder.AddField("2 раунд", $"{operatorRole.Mention}: {users.Users[3].Mention}\n{scannerRole.Mention}: {users.Users[1].Mention}\n{supressorRole.Mention}: {users.Users[5].Mention}");
                    embedBuilder.AddField("3 раунд", $"{operatorRole.Mention}: {users.Users[4].Mention}\n{scannerRole.Mention}: {users.Users[2].Mention}\n{supressorRole.Mention}: {users.Users[0].Mention}");
                    embedBuilder.AddField("4 раунд", $"{operatorRole.Mention}: {users.Users[5].Mention}\n{scannerRole.Mention}: {users.Users[3].Mention}\n{supressorRole.Mention}: {users.Users[1].Mention}");
                    embedBuilder.AddField("5 раунд", $"{operatorRole.Mention}: {users.Users[0].Mention}\n{scannerRole.Mention}: {users.Users[4].Mention}\n{supressorRole.Mention}: {users.Users[2].Mention}");
                    embedBuilder.AddField("6 раунд", $"{operatorRole.Mention}: {users.Users[1].Mention}\n{scannerRole.Mention}: {users.Users[5].Mention}\n{supressorRole.Mention}: {users.Users[3].Mention}");
                    await ReplyAsync("", false, embedBuilder.Build());
                }
                else
                    await ReplyAsync("Группа не подходит под условия");
            }
        }
    }
}
