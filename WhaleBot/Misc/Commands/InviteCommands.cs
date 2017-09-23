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
    public class InviteCommands : ModuleBase
    {
        [Command("invite")]
        [Summary("Returns an invite for the bot")]
        public async Task InviteCommand()
        {
            await ReplyAsync("", false, new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = "Bot invite",
                Color = new Color(178, 224, 40),
                Description = "Click [here](https://discordapp.com/oauth2/authorize?client_id=332106465412251648&scope=bot&permissions=0x7FF7FC7F) to invite me to your server\n" +
                              "Click [here](https://discord.gg/UGuRC86) to join my server"

            }.WithUrl("http://heeeeeeeey.com/"));
        }
    }
}
