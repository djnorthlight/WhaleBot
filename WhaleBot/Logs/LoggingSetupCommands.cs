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
        [Command("log edit")]
        [Summary("Sets the edited messages log channel")]
        [RequireUserPermission]
        public async Task LogEditCommand(SocketTextChannel chan = null)
        {
            using(var db = new DatabaseContext())
            {
                bool WasNull = false;
                var setup = db.GuildLoggingSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
                if (setup == null)
                {
                    setup = new GuildLoggingSetup();
                    WasNull = true;
                }

                setup.GuildId = Context.Guild.Id;
                if (chan != null) setup.EditChannelId = chan.Id;
                else setup.EditChannelId = 0;

                if(WasNull) db.GuildLoggingSetups.Add(setup);
                db.SaveChanges();
            }
            await ReplyAsync(chan == null ? "Edit logging channel has been cleared!" : "Edit logging channel has been set to " + chan.Mention);
        }

        [Command("log delete")]
        [Summary("Sets deleted messages log channel")]
        [RequireUserPermission]
        public async Task LogDeleteCommand(SocketTextChannel chan = null)
        {
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
                if (chan != null) setup.RemoveChannelId = chan.Id;
                else setup.RemoveChannelId = 0;

                if (WasNull) db.GuildLoggingSetups.Add(setup);
                db.SaveChanges();
            }
            await ReplyAsync(chan == null ? "Delete logging channel has been cleared!" : "Delete logging channel has been set to " + chan.Mention);
        }
    }
}
