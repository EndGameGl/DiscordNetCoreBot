using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Modules.Commands
{
    public class DestinyInfoModule : ModuleBase<SocketCommandContext>
    {
        public DestinyInfoModule() { }

        [Command("wish")]
        public async Task FetchLastWishInfo(int wish)
        {
            if (wish > 0 && wish < 16)
            {
                EmbedBuilder builder = new EmbedBuilder();
                switch (wish)
                {
                    case 1:
                        builder.WithTitle("Wish 1: Wish to Feed an Addiction");
                        builder.WithDescription("Grants an Ethereal Key");
                        builder.WithImageUrl("https://d1lss44hh2trtw.cloudfront.net/assets/editorial/2018/09/destiny-2-wish-1-first-wish.jpg");
                        break;
                    case 2:
                        builder.WithTitle("Wish 2: A wish for material validation");
                        builder.WithDescription("Causes a chest to spawn between the Morgeth, the Spirekeeper fight and the Vault that can only be opened with a Glittering Key");
                        builder.WithImageUrl("https://d1lss44hh2trtw.cloudfront.net/assets/editorial/2018/09/destiny-2-wish-2-second-wish.jpg");
                        break;
                    case 3:
                        builder.WithTitle("Wish 3: A wish for others to celebrate your success");
                        builder.WithDescription("Unlocks an Emblem");
                        builder.WithImageUrl("https://d1lss44hh2trtw.cloudfront.net/assets/editorial/2018/09/destiny-2-wish-3-third-wish.jpg");
                        break;
                    case 4:
                        builder.WithTitle("Wish 4: A wish to look athletic and elegant");
                        builder.WithDescription("Will immediately wipe the Fireteam and teleport them to the beginning of the fight against __Shuro Chi, the Corrupted__");
                        builder.WithImageUrl("https://d1lss44hh2trtw.cloudfront.net/assets/editorial/2018/09/destiny-2-wish-4-fourth-wish.jpg");
                        break;
                    case 5:
                        builder.WithTitle("Wish 5: A wish for a promising future");
                        builder.WithDescription($"Will immediately wipe the Fireteam and teleport them to the beginning of the {Format.Underline("Morgeth, the Spirekeeper")}, fight");
                        builder.WithImageUrl("https://d1lss44hh2trtw.cloudfront.net/assets/editorial/2018/09/destiny-2-wish-5-fifth-wish.jpg");
                        break;
                    case 6:
                        builder.WithTitle("Wish 6: A wish to move the hands of time");
                        builder.WithDescription("Will immediately wipe the Fireteam and teleport them to the beginning of the Vault encounter");
                        builder.WithImageUrl("https://d1lss44hh2trtw.cloudfront.net/assets/editorial/2018/09/destiny-2-wish-6-sixth-wish.jpg");
                        break;
                    case 7:
                        builder.WithTitle("Wish 7: A wish to help a friend in need");
                        builder.WithDescription("Will immediately wipe the Fireteam and teleport them to the beginning of the Riven of a Thousand Voices encounter");
                        builder.WithImageUrl("https://d1lss44hh2trtw.cloudfront.net/assets/editorial/2018/09/destiny-2-wish-7-seventh-wish.jpg");
                        break;
                    case 8:
                        builder.WithTitle("Wish 8: A wish to stay here forever");
                        builder.WithDescription("Will play the song, Hope for the Future");
                        builder.WithImageUrl("https://d1lss44hh2trtw.cloudfront.net/assets/editorial/2018/09/destiny-2-wish-8-eighth-wish.jpg");
                        break;
                    case 9:
                        builder.WithTitle("Wish 9: A wish to stay here forever");
                        builder.WithDescription("Activates a piece of dialogue from Failsafe, who then speaks throughout the raid");
                        builder.WithImageUrl("https://d1lss44hh2trtw.cloudfront.net/assets/editorial/2018/09/destiny-2-wish-9-ninth-wish.jpg");
                        break;
                    case 10:
                        builder.WithTitle("Wish 10: A wish to stay here forever");
                        builder.WithDescription("Adds Drifter dialogue to the raid");
                        builder.WithImageUrl("https://d1lss44hh2trtw.cloudfront.net/assets/editorial/2018/10/destiny-2-wish-10-tenth-wish.jpg");
                        break;
                    case 11:
                        builder.WithTitle("Wish 11: A wish to stay here forever");
                        builder.WithDescription("Adds an explosive effect to headshot kills, similar to the Grunt Birthday Party skull from Halo 2");
                        builder.WithImageUrl("https://d1lss44hh2trtw.cloudfront.net/assets/editorial/2018/09/destiny-2-wish-11-eleventh-wish.jpg");
                        break;
                    case 12:
                        builder.WithTitle("Wish 12: A wish to open your mind to new ideas");
                        builder.WithDescription("Adds an effect around the player's head");
                        builder.WithImageUrl("https://d1lss44hh2trtw.cloudfront.net/assets/editorial/2018/09/destiny-2-wish-12-twelfth-wish.jpg");
                        break;
                    case 13:
                        builder.WithTitle("Wish 13: A wish for the means to feed an addiction");
                        builder.WithDescription("Enables Extinguish, where if one player dies, the entire Fireteam goes to Orbit and the raid will reset");
                        builder.WithImageUrl("https://d1lss44hh2trtw.cloudfront.net/assets/editorial/2018/10/destiny-2-wish-13-thirteenth-wish.jpg");
                        break;
                    case 14:
                        builder.WithTitle("Wish 14: A wish for love and support");
                        builder.WithDescription("Spawns several Corrupted Eggs throughout the raid");
                        builder.WithImageUrl("https://d1lss44hh2trtw.cloudfront.net/assets/editorial/2018/09/destiny-2-wish-14-fourteenth-wish.jpg");
                        break;
                    case 15:
                        builder.WithTitle("Wish 14: “This one you shall cherish.” – Riven of a Thousand Voices");
                        builder.WithDescription("Not found yet");
                        builder.WithImageUrl("https://www.vhv.rs/dpng/d/57-574294_old-man-shrugging-shoulders-meme-hd-png-download.png");
                        break;
                }
                await ReplyAsync("", false, builder.Build());
            }
            else
                await ReplyAsync("No wish with such number."); 
        }

        [Command("dsc stage one roles")]
        public async Task DeepStoneCrypt_ChallengeOneRoles()
        {

        }

        [Command("dsc stage two roles")]
        public async Task DeepStoneCrypt_ChallengeTwoRoles()
        {

        }

        [Command("dsc stage three roles")]
        public async Task DeepStoneCrypt_ChallengeThreeRoles(params string[] users)
        {
            if (users.Length < 6)
                await ReplyAsync("Недостаточно участников");
            await ReplyAsync(
                $"1. Оператор: {users[2]}; Сканер: {users[0]}; Подавитель: {users[4]}\n" +
                $"2. Оператор: {users[3]}; Сканер: {users[1]}; Подавитель: {users[5]}\n" +
                $"3. Оператор: {users[4]}; Сканер: {users[2]}; Подавитель: {users[0]}\n" +
                $"4. Оператор: {users[5]}; Сканер: {users[3]}; Подавитель: {users[1]}\n" +
                $"5. Оператор: {users[0]}; Сканер: {users[4]}; Подавитель: {users[2]}\n" +
                $"6. Оператор: {users[1]}; Сканер: {users[5]}; Подавитель: {users[3]}");
        }

        [Command("dsc stage four roles")]
        public async Task DeepStoneCrypt_ChallengeFourRoles(params string[] users)
        {
            if (users.Length < 6)
                await ReplyAsync("Недостаточно участников");
            await ReplyAsync(
                $"1 ядро. Операторы ({users[0]}, {users[1]}):  1-5\n" +
                $"2 ядро. Подавители ({users[2]}, {users[3]}): 2-6\n" +
                $"3 ядро. {users[4]}: 3-5\n" +
                $"4 ядро. {users[5]}: 4-6");
        }
    }
}
