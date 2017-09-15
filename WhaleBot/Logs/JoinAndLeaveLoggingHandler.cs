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
    public class JoinAndLeaveLoggingHandler
    {
        private DiscordSocketClient client;
        public JoinAndLeaveLoggingHandler(DiscordSocketClient client)
        {
            this.client = client;
            client.UserJoined += Client_UserJoined;
            client.UserLeft += Client_UserLeft;
        }

        private async Task Client_UserLeft(SocketGuildUser arg)
        {
            GuildLoggingSetup setup;
            using (var db = new DatabaseContext()) setup = db.GuildLoggingSetups.FirstOrDefault(x => x.GuildId == arg.Guild.Id);


            await arg.Guild.GetTextChannel(setup.LeaveChannelId).SendMessageAsync("", false, new EmbedBuilder
            {
                Title = "User left!",
                Description = $"User {arg.Mention} ({arg.ToString()}) left",
                Color = new Color(178, 224, 40),
            }.WithUrl("http://heeeeeeeey.com/"));
        }

        private async Task Client_UserJoined(SocketGuildUser arg)
        {
            GuildLoggingSetup setup;
            using (var db = new DatabaseContext()) setup = db.GuildLoggingSetups.FirstOrDefault(x => x.GuildId == arg.Guild.Id);


            await arg.Guild.GetTextChannel(setup.JoinChannelId).SendMessageAsync("", false, new EmbedBuilder
            {
                Title = "User joined!",
                Description = $"User {arg.Mention} ({arg.ToString()}) joined",
                Color = new Color(178, 224, 40),
            }.WithUrl("http://heeeeeeeey.com/"));
        }

    }
}
