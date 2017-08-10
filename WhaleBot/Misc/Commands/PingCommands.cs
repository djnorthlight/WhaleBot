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
    public class PingCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")][Summary("Returns the bots ping")]
        public async Task PingCommand()
        {
            await ReplyAsync(Context.Client.Latency.ToString() + "ms");
        }
    }
}
