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
    public class AgreeCommands : ModuleBase<SocketCommandContext>
    {
        [Command("agree")][Remarks("Exclude from help")][RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task AgreeCommand()
        {
            if (Context.Guild.Id == 293182220737445899 && !(Context.User as SocketGuildUser).Roles.Any(x => x.Id == 293212793258246145))
                await (Context.User as SocketGuildUser).AddRoleAsync(Context.Guild.GetRole(293212793258246145));
        }
    }
}
