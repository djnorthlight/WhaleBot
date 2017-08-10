using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.IO;
using System.Diagnostics;

namespace DiscordSelfuBot
{
    public class SayCommands : ModuleBase
    {
        [Command("say")]
        [RequireOwner]
        public async Task SayCommand(ulong id, [Remainder]string text)
        {
            await Context.Message.DeleteAsync();
            var user = await Context.Guild.GetUserAsync(id);
            var dm = await user.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync(text);
        }
        [Command("say")]
        [RequireOwner]
        public async Task SayCommand([Remainder]string text)
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(text);
        }
        [Command("say tts")][Priority(2)]
        [RequireOwner]
        public async Task SayTtsCommands([Remainder]string text)
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(text, true);
        }
    }

}
