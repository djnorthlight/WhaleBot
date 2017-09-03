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
    public class OverwatchCommands : ModuleBase<SocketCommandContext>
    {
        [Command("overwatch")][WhizlSpecific][RequireUserPermission]
        public async Task OverwatchCommand()
        {
            IUserMessage mess;
            if ((Context.User as IGuildUser).RoleIds.Contains<ulong>(353978491777056778))
            {
                await (Context.User as SocketGuildUser).RemoveRoleAsync(Context.Guild.GetRole(353978491777056778));
                mess = await ReplyAsync("Overwatch is off 👍");
            }
            else
            {
                await (Context.User as SocketGuildUser).AddRoleAsync(Context.Guild.GetRole(353978491777056778));
                mess = await ReplyAsync("Overwatch is on 👍");
            }

            var _ = Task.Run(async () =>
            {
                await Task.Delay(5000);
                await mess.DeleteAsync();
                await Context.Message.DeleteAsync();
            });

        }
    }
}
