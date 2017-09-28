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
        public async Task MuteCommand([Remainder]SocketGuildUser mutee)
        {
            GuildSetup setup;
            using(var db = new DatabaseContext())
            {
                db.CachedRoles.Add(new CachedRoles { RoleIds = (mutee as IGuildUser).RoleIds.Where(x => x != Context.Guild.EveryoneRole.Id).ToList(), UserId = mutee.Id });
                setup = db.GuildSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
                db.SaveChanges();
            }

            await mutee.RemoveRolesAsync(mutee.Roles.Where(x => x.IsEveryone == false));
            await mutee.AddRoleAsync(Context.Guild.GetRole(setup.MutedRoleId));

            await ReplyAsync($"Muted {mutee.Mention} 👌");
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
            await mutee.RemoveRoleAsync(Context.Guild.GetRole(setup.MutedRoleId));

            await ReplyAsync($"Unmuted {mutee.Mention} 👌");
        }
    }
}
