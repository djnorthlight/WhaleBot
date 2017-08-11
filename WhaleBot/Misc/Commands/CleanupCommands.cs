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
        CommandService commands;
        public CleanupCommands(CommandService commands)
        {
            this.commands = commands;
        }

        [Command("cleanup")]
        [Summary("Cleans the channel from bots")]
        [RequireUserPermission]
        public async Task CleanupCommand()
        {
            List<string> prefixes = new List<string> { "-", "?", ".", "!", "--", ":", ";" , "//", "/", "!!", "??", "bb", ">"};
            var delete = await Context.Channel.GetMessagesAsync(100).Flatten();
            
            var deletee = delete.Where(x => x.Author.IsBot).ToList();
            var PossibleCommand = false;
            IMessage PossibleCommandMessage = null;
            foreach(var mess in delete.OrderBy(x => x.Timestamp))
            {
                if (PossibleCommand) if(mess.Author.IsBot) deletee.Add(PossibleCommandMessage);
                PossibleCommand = false;
                PossibleCommandMessage = null;

                if (prefixes.Any(x => mess.Content.ToLower().StartsWith(x)))
                {
                    PossibleCommand = true;
                    PossibleCommandMessage = mess;
                }
            }
            await Context.Channel.DeleteMessagesAsync(deletee);

            var t = Task.Run(async () =>
            {
                await Task.Delay(2000);
                await Context.Message.DeleteAsync();
            });

        }
    }
}
