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
    public class HelpCommands : ModuleBase<SocketCommandContext>
    {
        private CommandService CommandService;
        public HelpCommands(CommandService CommandService)
        {
            this.CommandService = CommandService;
        }

        [Command("help")][Summary("Displays this message")]
        public async Task HelpCommand()
        {
           var embed =  new EmbedBuilder
           {
               Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
               Title = "Help",
               Color = new Color(178, 224, 40),
           }.WithUrl("http://heeeeeeeey.com/");

            foreach(var Command in CommandService.Commands)
            {
                if(!Command.Preconditions.Contains(new RequireOwnerAttribute()) && !Command.Preconditions.Contains(new WhizlSpecificAttribute())&& Command.Remarks != "Exclude from help") embed.AddField(new EmbedFieldBuilder { Name = $"{string.Join(", ", Command.Aliases)}", Value = Command.Summary ?? "Cygan you cunt theres no summary here"});                
            }


            await Context.User.SendMessageAsync("", false, embed);
        }
    }
}
