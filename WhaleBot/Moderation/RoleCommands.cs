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
    public class RoleCommands : ModuleBase<SocketCommandContext>
    {
        [Command("role")]
        [RequireUserPermission]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Remarks("Exclude from help")]
        [Priority(2)]
        public async Task RoleCommand(SocketGuildUser user, [Remainder]SocketRole role) => await RoleCommand(user, role);

        [Command("role")]
        [RequireUserPermission]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Summary("Adds/removes roles\n**Syntax**: `role User Role(s)`")]
        public async Task RoleCommand(SocketGuildUser user, params SocketRole[] roles)
        {
            var addRoles = new List<SocketRole>();
            var removeRoles = new List<SocketRole>();

            foreach(SocketRole role in roles)
            {
                if (user.Roles.Contains(role)) removeRoles.Add(role);
                else addRoles.Add(role);
            }

            await user.AddRolesAsync(addRoles, new RequestOptions { AuditLogReason = $"Added by {Context.User.ToString()}" });
            await user.RemoveRolesAsync(removeRoles, new RequestOptions { AuditLogReason = $"Removed by {Context.User.ToString()}" });

            List<string> addedRoles = new List<string>();
            List<string> removedRoles = new List<string>();

            foreach (SocketRole role in addRoles) addedRoles.Add(role.Name);
            foreach (SocketRole role in removeRoles) removedRoles.Add(role.Name);

            var added = addRoles.Count > 0 ? "Added: " : "";
            var space = addRoles.Count > 0 && removeRoles.Count > 0 ? " " : "";
            var removed = removeRoles.Count > 0 ? "Removed: " : "";

            var reply = await ReplyAsync($"Done. {added}{string.Join(", ", addedRoles)}{space}{removed}{string.Join(", ", removedRoles)}");

            var t = Task.Run(async () =>
            {
                await Task.Delay(15000);
                await reply.DeleteAsync();
                await Context.Message.DeleteAsync();
            });
        }
    }
}
