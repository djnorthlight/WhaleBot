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
using System.Threading;

namespace WhaleBot
{
    public class BlacklistedEmojiHandler : ModuleBase<SocketCommandContext>
    {
        DiscordSocketClient client;
        public BlacklistedEmojiHandler(DiscordSocketClient client)
        {
            this.client = client;
            client.MessageReceived += Client_MessageReceived;
        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            var guild = (arg.Channel as SocketGuildChannel).Guild;
            if (guild.Id == 324282875035779072 && arg.ToString().ToLower().Contains("<:gw") && guild.CurrentUser.Nickname == "WhalyBot")
            {
                await arg.DeleteAsync();
            }
        }
    }
}
