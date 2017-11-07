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
        [RequireUserPermission]
        [Summary("Kicks a user\n**Syntax**: `kick User Reason`")]
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

            await user.KickAsync($"Kicked by {Context.User.Username}: {reason}");

            await ReplyAsync($"Kicked **{user.ToString()}** (`{reason}`) 👌");

            try
            {
                await user.SendMessageAsync("", false, new EmbedBuilder
                {
                    Title = "Looks like you fucked up",
                    Description = $"You've been kicked from {Context.Guild.Name}",
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder { Name = "Reason", Value = reason, IsInline = true  },
                        new EmbedFieldBuilder { Name = "Kicked by", Value = Context.User.ToString(), IsInline = true  }
                    },
                    Color = new Color(178, 224, 40),
                    Timestamp = DateTime.Now,
                    ThumbnailUrl = new StringBuilder(Context.Guild.IconUrl).Replace("jpg", "webp").ToString()
                }.WithUrl("http://heeeeeeeey.com/"));
            } catch { }

            using(var db = new DatabaseContext())
            {
                var modChannelId = db.GuildSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id)?.ModChannelId;
                if (modChannelId != 0)
                {
                    await Context.Guild.GetTextChannel((ulong)modChannelId).SendMessageAsync("", false, new EmbedBuilder
                    {
                        Title = "User kicked",
                        Description = $"{user.ToString()} has been kicked",
                        ThumbnailUrl = user.GetAvatarUrl(),
                        Fields = new List<EmbedFieldBuilder>
                        {
                            new EmbedFieldBuilder { Name = "Reason", Value = reason, IsInline = true },
                            new EmbedFieldBuilder { Name = "Kicked by", Value = Context.User.Mention, IsInline = true },
                        },
                        Color = new Color(178, 224, 40),
                        Timestamp = DateTime.Now,
                    }.WithUrl("http://heeeeeeeey.com/"));
                }
            }

            using (var db = new DatabaseContext())
            {
                db.Infractions.Add(new Infraction(Context.Guild.Id, Context.User.Id, user.Id, reason, InfractionType.Kick));
                await db.SaveChangesAsync();
            }
        }
    }
}
