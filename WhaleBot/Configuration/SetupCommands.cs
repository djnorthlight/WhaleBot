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
    public class SetupCommands : ModuleBase<SocketCommandContext>
    {
        [Command("setup")]
        [Summary("Sets the configuration")]
        [RequireUserPermission]
        public async Task SetupCommand(string node, SocketTextChannel chan = null)
        {
            bool WasFailed = false;
            using (var db = new DatabaseContext())
            {
                bool WasNull = false;
                var setup = db.GuildSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
                if (setup == null)
                {
                    setup = new GuildSetup();
                    WasNull = true;
                }

                setup.GuildId = Context.Guild.Id;

                switch (node.ToLower())
                {
                    case "edit_log":
                        if (chan != null) setup.EditChannelId = chan.Id;
                        else setup.EditChannelId = 0;
                        break;
                    case "delete_log":
                        if (chan != null) setup.RemoveChannelId = chan.Id;
                        else setup.RemoveChannelId = 0;
                        break;
                    case "join_log":
                        if (chan != null) setup.JoinChannelId = chan.Id;
                        else setup.RemoveChannelId = 0;
                        break;
                    case "leave_log":
                        if (chan != null) setup.LeaveChannelId = chan.Id;
                        else setup.RemoveChannelId = 0;
                        break;
                    case "mod_log":
                        if (chan != null) setup.ModChannelId = chan.Id;
                        else setup.RemoveChannelId = 0;
                        break;
                    default:
                        await ReplyAsync("You fucked up");
                        WasFailed = true;
                        break;
                }


                if(WasNull) db.GuildSetups.Add(setup);
                db.SaveChanges();
            }
            var firstLetter = node.ToCharArray().First().ToString().ToUpper();
            var nodee = firstLetter + node.Substring(1, (node.IndexOf('_') - 1));

            if (!WasFailed)await ReplyAsync(chan == null ? $"{nodee} channel has been cleared!" : $"{nodee} logging channel has been set to {chan.Mention}");
        }

        [Command("setup")]
        [Summary("Sets the configuration")][Remarks("Exclude from help")]
        [RequireUserPermission]
        public async Task SetupCommand(string node, SocketRole role = null)
        {
            bool WasFailed = false;
            using (var db = new DatabaseContext())
            {
                bool WasNull = false;
                var setup = db.GuildSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
                if (setup == null)
                {
                    setup = new GuildSetup();
                    WasNull = true;
                }

                setup.GuildId = Context.Guild.Id;

                switch (node.ToLower())
                {
                    case "muted_role":
                        if (role != null) setup.MutedRoleId = role.Id;
                        else setup.MutedRoleId = 0;
                        break;
                    default:
                        await ReplyAsync("You fucked up");
                        WasFailed = true;
                        break;
                }


                if (WasNull) db.GuildSetups.Add(setup);
                db.SaveChanges();
            }
            var firstLetter = node.ToCharArray().First().ToString().ToUpper();
            var nodee = firstLetter + node.Substring(1, (node.IndexOf('_') - 1 ));

            if (!WasFailed) await ReplyAsync(role == null ? $"{nodee} role has been cleared!" : $"{nodee} role has been set to {role.Name}");
        }
    }
}
