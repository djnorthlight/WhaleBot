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
    public class TosMessageHandler
    {
        DiscordSocketClient client;
        public TosMessageHandler(DiscordSocketClient client)
        {
            this.client = client;
            client.JoinedGuild += Client_JoinedGuild;
        }

        private async Task Client_JoinedGuild(SocketGuild arg)
        {
            var tos = File.ReadAllText(@"Misc\tos.txt");
            var chan = arg.TextChannels.FirstOrDefault(x => x.Name == "general");
            if (chan == null) chan = await arg.Owner.GetOrCreateDMChannelAsync() as SocketTextChannel;
            IDMChannel dmchan = null;

            try { await chan.SendMessageAsync(tos); } catch { dmchan = await arg.Owner.GetOrCreateDMChannelAsync(); }
            try { await dmchan.SendMessageAsync(tos); } catch { if(dmchan != null) await arg.LeaveAsync(); }
        }
    }
}
