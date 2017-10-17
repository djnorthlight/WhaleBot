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

namespace WhaleBot.Moderation
{
    public class WarnCommands : ModuleBase<SocketCommandContext>
    {
        [Command("warn")][RequireUserPermission][Remarks("Warns a user")]
        public async Task MuteCommand(SocketGuildUser user, [Remainder]string reason)
        {
            GuildSetup setup;
            using (var db = new DatabaseContext())
            {
                setup = db.GuildSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id);

                if (Context.User == user)
                {
                    await ReplyAsync("What are you drunk?");
                    return;
                }

                if (user.Roles.Max(x => x.Position) >= (Context.User as SocketGuildUser).Roles.Max(x => x.Position))
                {
                    await ReplyAsync("You can only warn people who have a lower role than you");
                    return;
                }
            }

            try
            {
                await user.SendMessageAsync("", false, new EmbedBuilder
                {
                    Title = "Looks like you might have done something wrong",
                    Description = $"You've been warned on {Context.Guild.Name}",
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder { Name = "Reason", Value = reason, IsInline = true  },
                        new EmbedFieldBuilder { Name = "Warned by", Value = Context.User.ToString(), IsInline = true  },
                    },
                    Color = new Color(178, 224, 40),
                    Timestamp = DateTime.Now,
                    ThumbnailUrl = new StringBuilder(Context.Guild.IconUrl).Replace("jpg", "webp").ToString()
                }.WithUrl("http://heeeeeeeey.com/"));
            }
            catch { }

            var modChannelId = setup?.ModChannelId;
            if (modChannelId != 0)
            {
                await Context.Guild.GetTextChannel((ulong)modChannelId).SendMessageAsync("", false, new EmbedBuilder
                {
                    Title = "User warned",
                    Description = $"{user.ToString()} has been warned",
                    ThumbnailUrl = user.GetAvatarUrl(),
                    Fields = new List<EmbedFieldBuilder>
                        {
                            new EmbedFieldBuilder { Name = "Reason", Value = reason, IsInline = true },
                            new EmbedFieldBuilder { Name = "Warned by", Value = Context.User.Mention, IsInline = true },
                        },
                    Color = new Color(178, 224, 40),
                    Timestamp = DateTime.Now,
                }.WithUrl("http://heeeeeeeey.com/"));
            }


            var inf = new Infraction(Context.Guild.Id, Context.User.Id, user.Id, reason, InfractionType.Warn);
            using (var db = new DatabaseContext())
            {
                db.Infractions.Add(inf);
                await db.SaveChangesAsync();
            }

            await ReplyAsync($"Warned **{user.ToString()}** (`{reason}`) 👌");
        }
    }
}
