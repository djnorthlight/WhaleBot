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
    public class LoggingSetupCommands : ModuleBase<SocketCommandContext>
    {
        [Command("log")]
        [Summary("Sets the edits/deletes/joins/leaves log channel")]
        [RequireUserPermission]
        public async Task LogEditCommand(string node, SocketTextChannel chan = null)
        {
            bool WasFailed = false;
            using (var db = new DatabaseContext())
            {
                bool WasNull = false;
                var setup = db.GuildLoggingSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
                if (setup == null)
                {
                    setup = new GuildLoggingSetup();
                    WasNull = true;
                }

                setup.GuildId = Context.Guild.Id;

                switch (node.ToLower())
                {
                    case "edit":
                        if (chan != null) setup.EditChannelId = chan.Id;
                        else setup.EditChannelId = 0;
                        break;
                    case "delete":
                        if (chan != null) setup.RemoveChannelId = chan.Id;
                        else setup.RemoveChannelId = 0;
                        break;
                    case "join":
                        if (chan != null) setup.JoinChannelId = chan.Id;
                        else setup.RemoveChannelId = 0;
                        break;
                    case "leave":
                        if (chan != null) setup.LeaveChannelId = chan.Id;
                        else setup.RemoveChannelId = 0;
                        break;
                    default:
                        await ReplyAsync("You fucked up");
                        WasFailed = true;
                        break;
                }


                if(WasNull) db.GuildLoggingSetups.Add(setup);
                db.SaveChanges();
            }
            var firstLetter = node.ToCharArray().First().ToString().ToUpper();
            var nodee = firstLetter + node.Substring(1);

            if(!WasFailed)await ReplyAsync(chan == null ? $"{nodee} logging channel has been cleared!" : $"{nodee} logging channel has been set to {chan.Mention}");
        }
    }
}
