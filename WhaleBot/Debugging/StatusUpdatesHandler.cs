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
    public class StatusUpdatesHandler
    {
        private DiscordSocketClient client;
        private SocketTextChannel StatusChannel;
        private DateTime DisconnectedTime;
        public StatusUpdatesHandler(DiscordSocketClient client)
        {

            this.client = client;
            client.Ready += Client_Ready;
            client.Disconnected += Client_Disconnected;
        }

        private Task Client_Disconnected(Exception arg)
        {
            this.DisconnectedTime = DateTime.Now;
            return Task.CompletedTask;
        }

        private async Task Client_Ready()
        {
            this.StatusChannel = client.GetGuild(344200376070832152).GetTextChannel(344211493912444929);
            var time = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
            if (DisconnectedTime.Year == 1)
            {             
                await StatusChannel.SendMessageAsync($"I am ready, started in {time.Seconds}.{time.Milliseconds} seconds");
                return;
            }
            time = DateTime.Now - DisconnectedTime;

            await StatusChannel.SendMessageAsync($"I am ready again, reconnnected in {time.Seconds}.{time.Milliseconds} seconds");
        }
    }
}
