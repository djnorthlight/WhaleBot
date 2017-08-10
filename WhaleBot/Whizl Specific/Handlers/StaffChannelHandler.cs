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
    public class StaffChannelHandler : ModuleBase<SocketCommandContext>
    {
        private readonly Timer _timer;
        private readonly DiscordSocketClient client;
        private int StaffMessages;

        public StaffChannelHandler(DiscordSocketClient client)
        {
            this.client = client;
            client.MessageUpdated += Client_MessageUpdated;
            client.MessageReceived += Client_MessageReceived;
            int.TryParse(File.ReadAllText(@"\\OLIWIER-PC\ssl log\staff.txt"), out StaffMessages);


            _timer = new Timer(async _ =>
            {
                await DeleteMessages();
            },
            null,
            TimeSpan.FromHours(12),
            TimeSpan.FromHours(12));
        }

        private Task Client_MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            if (arg1.Value == null) return Task.CompletedTask;
            if (arg3.Id == 341230361251938315 && arg1.Value.IsPinned && !arg2.IsPinned) File.WriteAllText(@"\\OLIWIER-PC\ssl log\staff.txt", (StaffMessages + 1).ToString());
            StaffMessages++;
            return Task.CompletedTask;
        }


        private async Task DeleteMessages()
        {
            if (StaffMessages == 0) return;
            var chan = client.GetGuild(324282875035779072).GetTextChannel(341230361251938315);
            var mess = await chan.GetMessagesAsync(StaffMessages).Flatten();
            var npmess = mess.Where(x => !x.IsPinned);
            await chan.DeleteMessagesAsync(npmess);

            StaffMessages = 0;
            File.WriteAllText(@"\\OLIWIER-PC\ssl log\staff.txt", 0.ToString());
        }

        private Task Client_MessageReceived(SocketMessage arg)
        {
            if (arg.Channel.Id == 341230361251938315) File.WriteAllText(@"\\OLIWIER-PC\ssl log\staff.txt", (StaffMessages + 1).ToString());
            StaffMessages++;
            return Task.CompletedTask;
        }


        [Command("staff clear")][WhizlSpecific][RequireUserPermission]
        public async Task StaffClear()
        {
            await DeleteMessages();
        }
    }
}
