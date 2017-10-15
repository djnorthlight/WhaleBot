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

    public class MuteCommands : ModuleBase<SocketCommandContext>
    {
        private ExpiredInfractionsHandler handler;
        public MuteCommands(ExpiredInfractionsHandler handler)
        {
            this.handler = handler;
        }

        [Command("mute")][RequireUserPermission][Summary("Mutes a user")]
        public async Task MuteCommand(SocketGuildUser mutee, [Remainder]string reason) => await MuteCommand(mutee, null, reason);

        [Command("mute")][RequireUserPermission][Remarks("Exclude from help")]
        public async Task MuteCommand(TimeSpan? time, SocketGuildUser mutee,  [Remainder]string reason) => await MuteCommand(mutee, time, reason);

        [Command("mute")][RequireUserPermission][Remarks("Exclude from help")]
        public async Task MuteCommand(SocketGuildUser mutee, TimeSpan? time, [Remainder]string reason)
        {
            GuildSetup setup;
            using(var db = new DatabaseContext())
            {
                setup = db.GuildSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id);

                if (Context.User == mutee)
                {
                    await ReplyAsync("What are you drunk?");
                    return;
                }

                if (mutee.Roles.Max(x => x.Position) >= (Context.User as SocketGuildUser).Roles.Max(x => x.Position))
                {
                    await ReplyAsync("You can only kick people who have a lower role than you");
                    return;
                }

                if(mutee.Roles.Where(x => !x.IsEveryone).Count() == 1 && mutee.Roles.Any(x => x.Id == setup.MutedRoleId))
                {
                    await ReplyAsync($"**{mutee.ToString()}** is already muted");
                    return;
                }

                db.CachedRoles.Add(new CachedRoles { RoleIds = (mutee as IGuildUser).RoleIds.Where(x => x != Context.Guild.EveryoneRole.Id).ToList(), UserId = mutee.Id });
                
                await db.SaveChangesAsync();
            }

            await mutee.RemoveRolesAsync(mutee.Roles.Where(x => x.IsEveryone == false));
            await mutee.AddRoleAsync(Context.Guild.GetRole(setup.MutedRoleId), new RequestOptions { AuditLogReason = $"Muted by {Context.User.Username}: {reason}" });

            try
            {
                await mutee.SendMessageAsync("", false, new EmbedBuilder
                {
                    Title = "Looks like you did something wrong",
                    Description = $"You've been muted on {Context.Guild.Name}",
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder { Name = "Reason", Value = reason, IsInline = true  },
                        new EmbedFieldBuilder { Name = "Muted by", Value = Context.User.ToString(), IsInline = true  },
                        new EmbedFieldBuilder { Name = "Duration", Value = time.ToReadable() }
                    },
                    Color = new Color(178, 224, 40),
                    Timestamp = DateTime.Now,
                    ThumbnailUrl = new StringBuilder(Context.Guild.IconUrl).Replace("jpg", "webp").ToString()
                }.WithUrl("http://heeeeeeeey.com/"));
            }
            catch { }

            using (var db = new DatabaseContext())
            {
                var modChannelId = db.GuildSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id)?.ModChannelId;
                if (modChannelId != 0)
                {
                    await Context.Guild.GetTextChannel((ulong)modChannelId).SendMessageAsync("", false, new EmbedBuilder
                    {
                        Title = "User muted",
                        Description = $"{mutee.ToString()} has been muted",
                        ThumbnailUrl = mutee.GetAvatarUrl(),
                        Fields = new List<EmbedFieldBuilder>
                        {
                            new EmbedFieldBuilder { Name = "Reason", Value = reason, IsInline = true },
                            new EmbedFieldBuilder { Name = "Muted by", Value = Context.User.Mention, IsInline = true },
                            new EmbedFieldBuilder { Name = "Duration", Value = time.ToReadable() }
                        },
                        Color = new Color(178, 224, 40),
                        Timestamp = DateTime.Now,
                    }.WithUrl("http://heeeeeeeey.com/"));
                }
            }

            var inf = new Infraction(Context.Guild.Id, Context.User.Id, mutee.Id, reason, InfractionType.Mute, time);
            using (var db = new DatabaseContext())
            {
                db.Infractions.Add(inf);
                await db.SaveChangesAsync();
            }

            if(time != null) handler.AddTempMute(inf);
            await ReplyAsync($"Muted **{mutee.ToString()}** {(time.ToReadable() != "Infinite" ? $"for {time.ToReadable()}" : " ")}(`{reason}`) 👌");
        }


        [Command("unmute")][RequireUserPermission][Summary("Unmutes a user")]
        public async Task UnmuteCommand([Remainder]SocketGuildUser mutee)
        {
            GuildSetup setup;
            CachedRoles roles;
            using (var db = new DatabaseContext())
            {
                roles = db.CachedRoles.FirstOrDefault(x => x.UserId == mutee.Id);
                setup = db.GuildSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
                db.CachedRoles.Remove(db.CachedRoles.FirstOrDefault(x => x.UserId == mutee.Id));
                foreach (var inf in db.Infractions.Where(x => x.OffenderId == mutee.Id && !x.IsExpired && x.Type == InfractionType.Mute)) inf.IsExpired = true;
                await db.SaveChangesAsync();
            }

            List<SocketRole> addroles = new List<SocketRole>();
            foreach(var role in roles.RoleIds)
            {
                addroles.Add(Context.Guild.GetRole(role));
            }

            await mutee.AddRolesAsync(addroles);
            await mutee.RemoveRoleAsync(Context.Guild.GetRole(setup.MutedRoleId), new RequestOptions { AuditLogReason = $"Unmuted by {Context.User.Username}"});

            await ReplyAsync($"Unmuted **{mutee.Mention}** 👌");
        }
    }
}
