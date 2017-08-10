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
    public class WhaleRoleRemovalHandler
    {
        private readonly Timer _timer;
        private readonly DiscordSocketClient client;

        public WhaleRoleRemovalHandler(DiscordSocketClient client)
        {
            this.client = client;

            _timer = new Timer(_ =>
            {
                using(var db = new DatabaseContext())
                {
                    foreach(var c in db.RoleAssignments)
                    {
                        if (c.HourGiven.AddHours(24) < DateTime.Now)
                        {
                            var guild = client.GetGuild(324282875035779072);
                            try
                            {
                                guild.GetUser(c.UserId).RemoveRoleAsync(guild.GetRole(338109400411537408), new RequestOptions { AuditLogReason = "The whale has ended" });
                                db.RoleAssignments.Remove(c);
                            } catch { }
                        }
                    }
                    db.SaveChanges();
                }
            },
            null,
            TimeSpan.FromMinutes(1),  
            TimeSpan.FromMinutes(10)); 
        }
    }
}