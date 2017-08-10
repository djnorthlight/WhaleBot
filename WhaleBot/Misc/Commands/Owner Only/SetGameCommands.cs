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

namespace WhaleBot
{
    public class SetGameCommands : ModuleBase<SocketCommandContext>
    {
        [Command("game")][RequireOwner]
        public async Task GameSetCommand([Remainder]string game)
        {
            await Context.Client.SetGameAsync(game);
            await ReplyAsync("", false, new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = Context.Message.Author.Username,
                    IconUrl = Context.Message.Author.GetAvatarUrl()
                },
                Title = "Vari",
                Description = $"Im now playing {game} 👍",
                ThumbnailUrl = "http://imgur.com/f1a0ssv.png",
                Color = new Color(0, 255, 0)

            });
        }
        [Command("stream")][RequireOwner]
        public async Task StreamSetCommand([Remainder]string stream)
        {
            await Context.Client.SetGameAsync(stream, "https://www.twitch.tv/zoosia", StreamType.Twitch);
            await ReplyAsync("", false, new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = Context.Message.Author.Username,
                    IconUrl = Context.Message.Author.GetAvatarUrl()
                },
                Title = "Vari",
                Description = $"Im now streaming {stream} 👍",
                ThumbnailUrl = "http://imgur.com/f1a0ssv.png",
                Color = new Color(0, 255, 0)

            });
        }
    }
}
