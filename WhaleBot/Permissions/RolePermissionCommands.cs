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
    public class RolePermissionCommands : ModuleBase
    {
        [Command("perm add")]
        [Summary("Adds a user/role permission to use a certain command\n**Syntax:** `perm add @Role/User Command` Where command is the full name of the command")]
        public async Task PermissionAddCommand(IRole role, [Remainder]string command)
        {
            using(var db = new DatabaseContext())
            {
                db.Permissions.Add(new Permission(role.Id, Context.Guild.Id, command, true));
                db.SaveChanges();
            }
            await ReplyAsync("", false, new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = "Permission added!",
                Color = new Color(178, 224, 40),
                Description = $"{role.Mention} now has access to {command}"
            }.WithUrl("http://heeeeeeeey.com/"));

        }

        [Command("perm remove")]
        [Summary("Removes a user/role permission to use a certain command\n**Syntax:** `perm remove @Role/User Command` Where command is the full name of the command")]
        public async Task PermissionRemoveCommand(IRole role, [Remainder]string command)
        {
            Permission perm;
            using(var db = new DatabaseContext())
            {
                perm = db.Permissions.FirstOrDefault(x => x.TargetId == role.Id && x.IsRole == true && x.GuildId == Context.Guild.Id && x.CommandName == command);
            }
            
            if (perm == null)
            {
                await ReplyAsync($"{role.Name} doesn't have that permission");
                return;
            }
            using(var db = new DatabaseContext())
            {
                try { db.Permissions.Remove(perm); } catch { await ReplyAsync($"{role.Mention} doesn't have that permission"); return; }
                db.SaveChanges();
            }
            
            await ReplyAsync("", false, new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = "Permission removed!",
                Color = new Color(178, 224, 40),
                Description = $"{role.Mention} no longer has access to {command}"
            }.WithUrl("http://heeeeeeeey.com/"));

        }
    }
}
