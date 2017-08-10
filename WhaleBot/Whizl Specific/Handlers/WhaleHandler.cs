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
    public class WhaleHandler
    {
        DiscordSocketClient client;
        public WhaleHandler(DiscordSocketClient client)
        {
            this.client = client;
            client.MessageReceived += Client_MessageReceived;
        }

        private Task Client_MessageReceived(SocketMessage arg)
        {
            
            if (arg.Channel.Name != "guests" || arg.Author.IsBot || arg.Content == "-agree") return Task.CompletedTask;
            var memberRole = (arg.Channel as IGuildChannel).Guild.Roles.FirstOrDefault(x => x.Name.ToLower() == "member");
            if (memberRole == null) return Task.CompletedTask;
            if (!(arg.Author as IGuildUser).RoleIds.Contains(memberRole.Id))
            {
                IMessage whale = null;
                var t = Task.Run(async () =>
                {
                    whale = await arg.Channel.SendMessageAsync($"{arg.Author.Mention} It's `-agree` not `{arg.Content}`");
                    await Task.Delay(10000);
                    await whale.DeleteAsync();
                });

            }
            return Task.CompletedTask;
        }
    }
}
