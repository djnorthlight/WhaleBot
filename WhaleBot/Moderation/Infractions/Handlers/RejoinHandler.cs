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
using System.Threading;

namespace WhaleBot
{
    public class RejoinHandler
    {
        DiscordSocketClient client;
        public RejoinHandler(DiscordSocketClient client)
        {
            this.client = client;
            client.UserJoined += Client_UserJoined;
        }

        private async Task Client_UserJoined(SocketGuildUser arg)
        {
            using(var db = new DatabaseContext())
            {
                if (db.Infractions.Any(x => x.OffenderId == arg.Id && x.Type == InfractionType.Mute && !x.IsExpired))
                {
                    await arg.AddRoleAsync(arg.Guild.GetRole(db.GuildSetups.FirstOrDefault(x => x.GuildId == arg.Guild.Id).MutedRoleId));
                }
            }
        }
    }
}
