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
   
    public class KickCommands : ModuleBase<SocketCommandContext>
    {
        [Command("kick")]
        public async Task KickCommand(SocketGuildUser user, [Remainder]string reason)
        {
            if (Context.User == user)
            {
                await ReplyAsync("What are you drunk?");
                return;
            }

            if (user.Roles.Max(x => x.Position) >= (Context.User as SocketGuildUser).Roles.Max(x => x.Position))
            {
                await ReplyAsync("You can only kick people who have a lower role than you");
                return;
            }

            await user.SendMessageAsync("", false, new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = $"You've been kicked from {Context.Guild.Name}",
                Description = $"With reason: {reason}",
                //Fields = new List<EmbedFieldBuilder> { new EmbedFieldBuilder { Name = "Reason", Value = reason } },
                Color = new Color(178, 224, 40),
            }.WithUrl("http://heeeeeeeey.com/"));

            await user.KickAsync($"Kicked by {Context.User.Username}: {reason}");
        }
    }
}
