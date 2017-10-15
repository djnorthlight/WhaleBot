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

    public class BanCommands : ModuleBase<SocketCommandContext>
    {
        private ExpiredInfractionsHandler handler;
        public BanCommands(ExpiredInfractionsHandler handler)
        {
            this.handler = handler;
        }

        [Command("ban")][RequireUserPermission][Summary("Bans a user\n**Syntax**: `ban User Reason`")]
        public async Task BanCommand(SocketGuildUser user, [Remainder]string reason) => await BanCommand(user, null, reason);

        [Command("ban")][RequireUserPermission][Remarks("Exclude from help")]
        public async Task BanCommand(TimeSpan? time, SocketGuildUser user, [Remainder]string reason) => await BanCommand(user, time, reason);

        [Command("ban")][RequireUserPermission][Remarks("Exclude from help")]
        public async Task BanCommand(SocketGuildUser user, TimeSpan? time, [Remainder]string reason)
        {
            if (Context.User == user)
            {
                await ReplyAsync("What are you drunk?");
                return;
            }

            if (user.Roles.Max(x => x.Position) >= (Context.User as SocketGuildUser).Roles.Max(x => x.Position))
            {
                await ReplyAsync("You can only ban people who have a lower role than you");
                return;
            }

            try
            {
                await user.SendMessageAsync("", false, new EmbedBuilder
                {
                    Title = "Looks like you fucked up real hard",
                    Description = $"You've been banned from {Context.Guild.Name}",
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder { Name = "Reason", Value = reason, IsInline = true  },
                        new EmbedFieldBuilder { Name = "Banned by", Value = Context.User.ToString(), IsInline = true  },
                        new EmbedFieldBuilder { Name = "Duration", Value = time.ToReadable() }
                    },
                    Color = new Color(178, 224, 40),
                    Timestamp = DateTime.Now,
                    ThumbnailUrl = new StringBuilder(Context.Guild.IconUrl).Replace("jpg", "webp").ToString()
                }.WithUrl("http://heeeeeeeey.com/"));
            } catch { }

            using (var db = new DatabaseContext())
            {
                var modChannelId = db.GuildSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id)?.ModChannelId;
                if (modChannelId != 0)
                {
                    await Context.Guild.GetTextChannel((ulong)modChannelId).SendMessageAsync("", false, new EmbedBuilder
                    {
                        Title = "User banned",
                        Description = $"{user.ToString()} has been banned",
                        ThumbnailUrl = user.GetAvatarUrl(),
                        Fields = new List<EmbedFieldBuilder>
                        {
                            new EmbedFieldBuilder { Name = "Reason", Value = reason, IsInline = true },
                            new EmbedFieldBuilder { Name = "Banned by", Value = Context.User.Mention, IsInline = true },
                            new EmbedFieldBuilder { Name = "Duration", Value = time.ToReadable() }
                        },
                        Color = new Color(178, 224, 40),
                        Timestamp = DateTime.Now,
                    }.WithUrl("http://heeeeeeeey.com/"));
                }
            }

            await Context.Guild.AddBanAsync(user, 0, $"Banned by {Context.User.Username}: {reason}");

            var inf = new Infraction(Context.Guild.Id, Context.User.Id, user.Id, reason, InfractionType.Ban, time);
            using (var db = new DatabaseContext())
            {
                db.Infractions.Add(inf);
                await db.SaveChangesAsync();
            }

            if (time != null) handler.AddTempBan(inf);

            await ReplyAsync($"Banned **{user.ToString()}** {(time.ToReadable() != "Infinite" ? $"{time.ToReadable()} " : "")}(`{reason}`) 👌");
        }
    }
}
