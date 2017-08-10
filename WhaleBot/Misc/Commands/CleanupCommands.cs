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
    public class CleanupCommands : ModuleBase
    {
        [Command("cleanup")]
        [Summary("Cleans the channel from bots")]
        [RequireUserPermission]
        public async Task CleanupCommand()
        {
            var delete = await Context.Channel.GetMessagesAsync(50).Flatten();
            var deletee = delete.Where(x => x.Author.IsBot);
            await Context.Channel.DeleteMessagesAsync(deletee);
        }
    }
}
