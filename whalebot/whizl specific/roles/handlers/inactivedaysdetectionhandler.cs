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
            if (arg.Author.IsBot) return Task.CompletedTask;
            if ((arg.Channel as SocketGuildChannel).Guild.Id != 324282875035779072) return Task.CompletedTask;
            if (arg.Channel.GetType() == typeof(SocketDMChannel)) return Task.CompletedTask;

            var gAuthor = arg.Author as SocketGuildUser;
            var gChannel = arg.Channel as SocketGuildChannel;
            using (var db = new DatabaseContext())
            {
                int[] days = { -7, -14, -21, -28 };

                if (db.LoggedMessages.Where(x => x.AuthorId == arg.Author.Id && x.Timestamp > DateTime.Now.AddDays(-7) && x.MessageId != arg.Id).Count() == 0)
                {
                    if (gAuthor.Roles.Any(x => x.Id == (ulong)RoleLevel.Hyperactive)) (arg.Author as SocketGuildUser).RemoveRoleAsync(gChannel.Guild.GetRole((ulong)RoleLevel.Hyperactive), new RequestOptions { AuditLogReason = "Was inactive for 7 days" });
                    if (gAuthor.Roles.Any(x => x.Id == (ulong)RoleLevel.Active)) (arg.Author as SocketGuildUser).RemoveRoleAsync(gChannel.Guild.GetRole((ulong)RoleLevel.Active), new RequestOptions { AuditLogReason = "Was inactive for 7 days" });
                    if (gAuthor.Roles.Any(x => x.Id == (ulong)RoleLevel.Frequent)) (arg.Author as SocketGuildUser).RemoveRoleAsync(gChannel.Guild.GetRole((ulong)RoleLevel.Frequent), new RequestOptions { AuditLogReason = "Was inactive for 7 days" });
                    if (gAuthor.Roles.Any(x => x.Id == (ulong)RoleLevel.Recognised)) (arg.Author as SocketGuildUser).RemoveRoleAsync(gChannel.Guild.GetRole((ulong)RoleLevel.Recognised), new RequestOptions { AuditLogReason = "Was inactive for 7 days" });
                } 
            }
            return Task.CompletedTask;
        }
    }
}
