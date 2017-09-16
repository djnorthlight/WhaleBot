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
    public class ProfileCommands : ModuleBase<SocketCommandContext>
    {
        [Command("profile")]
        [RequireUserPermission]
        [Summary("Shows information about a user")]
        public async Task ProfileCommand(SocketGuildUser userGiven = null)
        {
            var user = userGiven ?? Context.User as SocketGuildUser;

            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = $"{user.Username}'s info",
                Color = new Color(178, 224, 40),
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder { Name = "Username", Value = user.Username, IsInline = true },
                    new EmbedFieldBuilder { Name = "Discriminator", Value = user.Discriminator, IsInline = true },
                    new EmbedFieldBuilder { Name = "Nickname", Value = user.Nickname ?? "Not set", IsInline = true },
                    new EmbedFieldBuilder { Name = "Avatar URL", Value = $"[Click here]({user.GetAvatarUrl()})", IsInline = true},
                    new EmbedFieldBuilder { Name = "Highest role", Value = user.Roles.FirstOrDefault(x => x.Position == user.Roles.Max(y => y.Position)), IsInline = true },
                    new EmbedFieldBuilder { Name = "Joined at", Value = user.JoinedAt.Value.ToLocalTime(), IsInline = true },
                    new EmbedFieldBuilder { Name = "Created at", Value = user.CreatedAt.ToLocalTime(), IsInline = true },
                }
            }.WithUrl("http://heeeeeeeey.com/");

            await ReplyAsync("", false, embed);
        }
    }
}
