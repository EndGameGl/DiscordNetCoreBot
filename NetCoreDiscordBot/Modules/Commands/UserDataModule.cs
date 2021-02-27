using Discord.Commands;
using NetCoreDiscordBot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreDiscordBot.Modules.Commands
{
    [Group("userdata")]
    public class UserDataModule : ModuleBase<SocketCommandContext>
    {
        private readonly IUserDataService _service;
        public UserDataModule(IUserDataService service)
        {
            _service = service;
        }

        [Command("add")]
        public async Task AddKey(string key, string value)
        {
            var data = await _service.GetDataAsync(Context.User);
            data.Cache.Add(key, value);
            await _service.SaveAsync(data);
        }

        [Command("get")]
        public async Task GetKey(string key)
        {
            var data = await _service.GetDataAsync(Context.User);
            if (data.Cache.TryGetValue(key, out var value))
            {
                await ReplyAsync(value);
            }
            else
                await ReplyAsync("Couldn't find any data.");
        }
        [Command("remove")]
        public async Task RemoveKey(string key)
        {
            var data = await _service.GetDataAsync(Context.User);
            if (data.Cache.Remove(key))
            {
                await ReplyAsync($"Removed key: {key}.");
            }
            else
                await ReplyAsync("Failed to remove key.");
        }
    }
}
