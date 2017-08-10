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

namespace WhaleBot
{
    public class WelcomeMessage
    {
        DiscordSocketClient client;
        public WelcomeMessage(DiscordSocketClient client)
        {
            this.client = client;
            client.UserJoined += Client_UserJoined;
        }

        private async Task Client_UserJoined(SocketGuildUser arg)
        {
            if (arg.Guild.Id != 324282875035779072) return;
            var chan = await arg.GetOrCreateDMChannelAsync();
            await chan.SendMessageAsync($"Hi, welcome to {arg.Guild.Name} remember to read the rules and type `-agree` in the guests channel");
        }
    }
}
