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
    public class InactiveDaysDetectionHandler
    {
        DiscordSocketClient client;
        public InactiveDaysDetectionHandler(DiscordSocketClient client)
        {
            this.client = client;
            client.MessageReceived += Client_MessageReceived;
        }

        private Task Client_MessageReceived(SocketMessage arg)
        {
            if (arg.Channel.GetType() == typeof(SocketDMChannel)) return Task.CompletedTask;
            using(var db = new DatabaseContext())
            {
                if (db.LoggedMessages.Where(x => x.AuthorId == arg.Author.Id && x.Timestamp < DateTime.Now.AddDays(-7) && x.MessageId != arg.Id).Count() == 0)
                {
                    if ((arg.Author as SocketGuildUser).Roles.Any(x => x.Id == (ulong)RoleLevel.Hyperactive)) (arg.Author as SocketGuildUser).RemoveRoleAsync((arg.Channel as SocketGuildChannel).Guild.GetRole((ulong)RoleLevel.Hyperactive));
                    if ((arg.Author as SocketGuildUser).Roles.Any(x => x.Id == (ulong)RoleLevel.Active)) (arg.Author as SocketGuildUser).RemoveRoleAsync((arg.Channel as SocketGuildChannel).Guild.GetRole((ulong)RoleLevel.Active));
                    if ((arg.Author as SocketGuildUser).Roles.Any(x => x.Id == (ulong)RoleLevel.Frequent)) (arg.Author as SocketGuildUser).RemoveRoleAsync((arg.Channel as SocketGuildChannel).Guild.GetRole((ulong)RoleLevel.Frequent));
                    if ((arg.Author as SocketGuildUser).Roles.Any(x => x.Id == (ulong)RoleLevel.Recognised)) (arg.Author as SocketGuildUser).RemoveRoleAsync((arg.Channel as SocketGuildChannel).Guild.GetRole((ulong)RoleLevel.Recognised));
                } 
            }
            return Task.CompletedTask;
        }
    }
}
