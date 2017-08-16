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
                List<SocketRole> roles = new List<SocketRole>();
                foreach (var role in gAuthor.Roles.OrderByDescending(x => x.Position))
                {
                    var values = Enum.GetValues(typeof(RoleLevel)).Cast<ulong>().AsEnumerable();
                    if (values.Any(x => x == role.Id)) roles.Add(role);
                }

                //checking logged messages for user activity
                int counter = 1;
                foreach (var role in roles)
                {
                    if (db.LoggedMessages.Where(x => x.AuthorId == arg.Author.Id && x.Timestamp > DateTime.Now.AddDays(-(counter * 7)) && x.MessageId != arg.Id).Count() == 0)
                        if (gAuthor.Roles.Any(x => x.Id == role.Id))
                        {
                            if (role.Id == (ulong)RoleLevel.Recognised) gAuthor.RemoveRoleAsync(gChannel.Guild.GetRole((ulong)RoleLevel.Recognised));
                            gAuthor.RemoveRoleAsync(gChannel.Guild.GetRole(role.Id), new RequestOptions { AuditLogReason = "Was inactive for 7 days" });
                        }                                               
                    counter++;
                }                      
            }
            return Task.CompletedTask;
        }
    }
}
