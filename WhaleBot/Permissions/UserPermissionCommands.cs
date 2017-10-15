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
    public class UserPermissionCommands : ModuleBase
    {


        [Command("perm add")]
        [Remarks("Exclude from help")]
        public async Task PermissionAddCommand(IUser user, [Remainder]string command)
        {
            using (var db = new DatabaseContext())
            {
                db.Permissions.Add(new Permission(user.Id, Context.Guild.Id, command, false));
                await db.SaveChangesAsync();
            }

            await ReplyAsync("", false, new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = "Permission added!",
                Color = new Color(178, 224, 40),
                Description = $"{user.Mention} now has access to {command}",
            }.WithUrl("http://heeeeeeeey.com/"));
            
        }


        [Command("perm remove")]
        [Remarks("Exclude from help")]
        public async Task PermissionRemoveCommand(IUser user, [Remainder]string command)
        {
            Permission perm;
            using (var db = new DatabaseContext())
            {
                perm = db.Permissions.FirstOrDefault(x => x.TargetId == user.Id && x.IsRole == false && x.GuildId == Context.Guild.Id && x.CommandName == command);
            }
                
            if (perm == null)
            {
                await ReplyAsync($"{user.Username} doesn't have that permission");
                return;
            }
            using (var db = new DatabaseContext())
            {
                try { db.Permissions.Remove(perm); } catch { await ReplyAsync($"{user.Mention} doesn't have that permission"); return; }
                await db.SaveChangesAsync();
            }
                
            await ReplyAsync("", false, new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = "Permission removed!",
                Color = new Color(178, 224, 40),
                Description = $"{user.Mention} no longer has access to {command}"
            }.WithUrl("http://heeeeeeeey.com/"));
            
        }
    }
}
