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
            if (arg.Channel.GetType() == typeof(SocketDMChannel)) return Task.CompletedTask;
            if ((arg.Channel as SocketGuildChannel).Guild.Id != 324282875035779072) return Task.CompletedTask;

            using (var db = new DatabaseContext())
            {
                MemberRoleInfo info = null;
                if (!db.MemberRoleInfos.Any(x => x.UserId == arg.Author.Id)) db.MemberRoleInfos.Add(info = new MemberRoleInfo { UserId = arg.Author.Id, DaysActive = 0, NextDay = DateTime.Now.AddDays(1) });
                if(info == null)info = db.MemberRoleInfos.FirstOrDefault(x => x.UserId == arg.Author.Id);
                if (info.NextDay.Day == DateTime.Now.Day)
                {
                    info.NextDay.AddDays(1);
                    info.DaysActive++;
                }
                if (info.NextDay.Day < DateTime.Now.Day)
                {
                    info.NextDay = DateTime.Now;
                    info.DaysActive = 0;
                }

                switch (info.DaysActive)
                {
                    case 3:
                        if (!(arg.Author as SocketGuildUser).Roles.Any(x => x.Id == (ulong)RoleLevel.Recognised)) (arg.Author as SocketGuildUser).AddRoleAsync((arg.Channel as SocketGuildChannel).Guild.GetRole((ulong)RoleLevel.Recognised));
                        break;
                    case 5:
                        if (!(arg.Author as SocketGuildUser).Roles.Any(x => x.Id == (ulong)RoleLevel.Frequent)) (arg.Author as SocketGuildUser).AddRoleAsync((arg.Channel as SocketGuildChannel).Guild.GetRole((ulong)RoleLevel.Frequent));
                        break;
                    case 7:
                        if (!(arg.Author as SocketGuildUser).Roles.Any(x => x.Id == (ulong)RoleLevel.Active)) (arg.Author as SocketGuildUser).AddRoleAsync((arg.Channel as SocketGuildChannel).Guild.GetRole((ulong)RoleLevel.Active));
                        break;
                    case 14:
                        if (!(arg.Author as SocketGuildUser).Roles.Any(x => x.Id == (ulong)RoleLevel.Hyperactive)) (arg.Author as SocketGuildUser).AddRoleAsync((arg.Channel as SocketGuildChannel).Guild.GetRole((ulong)RoleLevel.Hyperactive));
                        break;
                }
                db.SaveChanges();
                return Task.CompletedTask;                                  
            }
        }
    }
}
