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
    public class ActiveDaysDetectionHandler
    {
        DiscordSocketClient client;
        public ActiveDaysDetectionHandler(DiscordSocketClient client)
        {
            this.client = client;
            client.MessageReceived += Client_MessageReceived;
        }

        //TODO: stuff

        private Task Client_MessageReceived(SocketMessage arg)
        {
            if (arg.Author.IsBot) return Task.CompletedTask;
            if (arg.Channel.GetType() == typeof(SocketDMChannel)) return Task.CompletedTask;
            if ((arg.Channel as SocketGuildChannel).Guild.Id != 324282875035779072) return Task.CompletedTask;

            using (var db = new DatabaseContext())
            {
                MemberRoleInfo info = null;
                if (!db.MemberRoleInfos.Any(x => x.UserId == arg.Author.Id)) { db.MemberRoleInfos.Add(new MemberRoleInfo { UserId = arg.Author.Id, DaysActive = 0, NextDay = DateTime.Now.AddDays(1) }); db.SaveChanges(); }
                info = db.MemberRoleInfos.FirstOrDefault(x => x.UserId == arg.Author.Id);
                if (info.NextDay.Day == DateTime.Now.Day)
                {
                    info.NextDay = DateTime.Now.AddDays(1);
                    info.DaysActive++;
                    db.SaveChanges();
                }
                if (info.NextDay.Day < DateTime.Now.Day)
                {
                    info.NextDay = DateTime.Now;
                    info.DaysActive = 0;
                }

                var gAuthor = arg.Author as SocketGuildUser;
                var gChannel = arg.Channel as SocketGuildChannel;

                switch (info.DaysActive)
                {
                    case 3:
                        if (!gAuthor.Roles.Any(x => x.Id == (ulong)RoleLevel.Recognised)) gAuthor.AddRoleAsync(gChannel.Guild.GetRole((ulong)RoleLevel.Recognised), new RequestOptions { AuditLogReason = "Was active for 3 days" });
                        break;
                    case 5:
                        if (!gAuthor.Roles.Any(x => x.Id == (ulong)RoleLevel.Frequent)) gAuthor.AddRoleAsync(gChannel.Guild.GetRole((ulong)RoleLevel.Frequent), new RequestOptions { AuditLogReason = "Was active for 5 days"});
                        break;
                    case 7:
                        if (!gAuthor.Roles.Any(x => x.Id == (ulong)RoleLevel.Active)) gAuthor.AddRoleAsync(gChannel.Guild.GetRole((ulong)RoleLevel.Active), new RequestOptions { AuditLogReason = "Was active for 7 days" });
                        break;
                    case 14:
                        if (!gAuthor.Roles.Any(x => x.Id == (ulong)RoleLevel.Hyperactive)) gAuthor.AddRoleAsync(gChannel.Guild.GetRole((ulong)RoleLevel.Hyperactive), new RequestOptions { AuditLogReason = "Was active for 14 days" });
                        break;
                }
                db.SaveChanges();
                return Task.CompletedTask;                                  
            }
        }
    }
}
