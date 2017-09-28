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
            var pages = new List<EmbedBuilder>();

            int currentPage = -1;

            foreach(var Command in CommandService.Commands)
            {
                if (!Command.Preconditions.Contains(new RequireOwnerAttribute()) && !Command.Preconditions.Contains(new WhizlSpecificAttribute()) && Command.Remarks != "Exclude from help") pages[currentPage].AddField(new EmbedFieldBuilder { Name = $"{string.Join(", ", Command.Aliases)}", Value = Command.Summary ?? "Cygan you cunt theres no summary here" });                
                if (currentPage == -1 || pages[currentPage].Fields.Count() >= 25)
                {
                    currentPage++;
                    pages.Add(new EmbedBuilder
                    {
                        Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                        Title = $"Help {currentPage + 1}",
                        Color = new Color(178, 224, 40),
                    }.WithUrl("http://heeeeeeeey.com/"));
                }
            }

            foreach(var page in pages) await Context.User.SendMessageAsync("", false, page);

        }
    }
}
