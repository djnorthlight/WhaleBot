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
    public class StarboardSetupCommands : ModuleBase<SocketCommandContext>
    {
        [Command("starboard channel")][Remarks("Exclude from help")][RequireUserPermission]
        public async Task StarboardChannelCommand(SocketTextChannel channel = null)
        {
            using(var db = new DatabaseContext())
            {
                var setup = db.GuildStarringSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
                if (channel != null)
                {
                    if (setup == null) db.GuildStarringSetups.Add(new GuildStarringSetup { GuildId = Context.Guild.Id, StarboardChannelId = channel.Id });
                    else setup.StarboardChannelId = channel.Id;
                    await ReplyAsync($"The starboard channel has been set to {channel.Mention}");
                    db.SaveChanges();
                    return;
                }
                else
                {
                    if (setup != null) setup.StarboardChannelId = 0;
                    await ReplyAsync("The starboard channel has been cleared");
                }
                db.SaveChanges();
            }
        }
        [Command("starboard number")][Remarks("Exclude from help")][RequireUserPermission]
        public async Task StarboardNumberCommand(int number = 0)
        {
            using (var db = new DatabaseContext())
            {
                var setup = db.GuildStarringSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
                if (number != 0)
                {
                    if (setup == null) db.GuildStarringSetups.Add(new GuildStarringSetup { GuildId = Context.Guild.Id, StarsRequired = number });
                    else setup.StarsRequired = number;
                    await ReplyAsync($"The stars required number has been set to {number}");
                    db.SaveChanges();
                    return;
                }
                else
                {
                    if (setup != null) setup.StarsRequired = 0;
                    await ReplyAsync("The stars required number has been cleared");
                }
                db.SaveChanges();
            }
        }
    }
}

