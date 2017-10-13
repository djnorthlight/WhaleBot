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
        [Command("mute")][RequireUserPermission][Summary("Mutes a user")]
        public async Task MuteCommand(SocketGuildUser mutee, [Remainder]string reason)
        {
            GuildSetup setup;
            using(var db = new DatabaseContext())
            {
                db.CachedRoles.Add(new CachedRoles { RoleIds = (mutee as IGuildUser).RoleIds.Where(x => x != Context.Guild.EveryoneRole.Id).ToList(), UserId = mutee.Id });
                setup = db.GuildSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
                db.SaveChanges();
            }

            await mutee.RemoveRolesAsync(mutee.Roles.Where(x => x.IsEveryone == false));
            await mutee.AddRoleAsync(Context.Guild.GetRole(setup.MutedRoleId), new RequestOptions { AuditLogReason = $"Muted by {Context.User.Username}: {reason}" });

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
                        },
                        Color = new Color(178, 224, 40),
                        Timestamp = DateTime.Now,
                    }.WithUrl("http://heeeeeeeey.com/"));
                }
            }

            await ReplyAsync($"Muted **{mutee.ToString()}** (`{reason}`) 👌");
        }


        [Command("unmute")]
        [RequireUserPermission]
        [Summary("Unmutes a user")]
        public async Task UnmuteCommand([Remainder]SocketGuildUser mutee)
        {
            GuildSetup setup;
            CachedRoles roles;
            using (var db = new DatabaseContext())
            {
                roles = db.CachedRoles.FirstOrDefault(x => x.UserId == mutee.Id);
                setup = db.GuildSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
                db.CachedRoles.Remove(db.CachedRoles.FirstOrDefault(x => x.UserId == mutee.Id));
                db.SaveChanges();
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
