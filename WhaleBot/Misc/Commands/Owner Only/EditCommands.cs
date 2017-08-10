using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace DiscordSelfuBot
{
    public class EditCommands : ModuleBase<SocketCommandContext>
    {
        [Command("edit")][RequireOwner]
        public async Task EditCommand(ulong id, [Remainder]string content)
        {
            await Context.Message.DeleteAsync();
            var mod = await Context.Channel.GetMessageAsync(id, CacheMode.AllowDownload);
            await ((IUserMessage)mod).ModifyAsync(x =>
            x.Content = content
            );
        }
    }
}
