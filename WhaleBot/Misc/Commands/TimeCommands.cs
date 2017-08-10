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
    
    public class Time : ModuleBase<SocketCommandContext>
    {
        [Command("uptime")]
        [Summary("Shows the uptime of the bot")]
        public async Task UptimeCommand()
        {
            var time = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();

            string timedays = time.Days > 0 ? time.Days > 1 ? time.Days + " days " : time.Days + " day " : "";
            string timehours = time.Hours > 0 ? time.Hours > 1 ? time.Hours + " hours " : time.Hours + " hour " : "";
            string timeminutes = time.Minutes > 0 ? time.Minutes > 1 ? time.Minutes + " minutes " : time.Minutes + " minute " : "";
            string timeseconds = time.Seconds > 0 ? time.Seconds > 1 ?  time.Seconds + " seconds" : time.Seconds + " second" : "";
            

            await ReplyAsync("", false, new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = "Uptime",
                Color = new Color(178, 224, 40),
                Description = timedays + timehours + timeminutes + timeseconds
            }.WithUrl("http://heeeeeeeey.com/"));
        }
    }
}
