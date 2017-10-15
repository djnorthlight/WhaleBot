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
    public class ExpiredInfractionsHandler
    {
        private List<Timer> InfractionTimers { get; set; }
        private DiscordSocketClient Client { get; set; }

        public ExpiredInfractionsHandler(DiscordSocketClient client)
        {
            this.Client = client;
            InfractionTimers = new List<Timer>();
            client.Ready += Client_Ready;
        }

        private Task Client_Ready()
        {
            using (var db = new DatabaseContext())
            {
                foreach (var Infraction in db.Infractions.Where(x => !x.IsExpired && x.Duration != null))
                {
                    switch (Infraction.Type)
                    {
                        case InfractionType.Ban:
                            AddTempBan(Infraction);
                            break;

                        case InfractionType.Mute:
                            AddTempMute(Infraction);
                            break;
                    }
                }
            }
            return Task.CompletedTask;
        }

        public void AddTempBan(Infraction Infraction)
        {

            InfractionTimers.Add(new Timer(async _ =>
            {
                using (var db = new DatabaseContext())
                {
                    var inf = db.Infractions.FirstOrDefault(x => x.Id == Infraction.Id);
                    if (inf.IsExpired == true) return;
                    var guild = Client.GetGuild(inf.GuildId);
                    await guild.RemoveBanAsync(inf.OffenderId);
                    inf.IsExpired = true;
                    await db.SaveChangesAsync();
                }
            },
            null,
            TimeSpan.FromMilliseconds((DateTime.Now - DateTime.Now.AddMilliseconds(-Infraction.Duration.Value.TotalMilliseconds)).TotalMilliseconds),
            TimeSpan.FromMilliseconds(Timeout.Infinite)));

        }

        public void AddTempMute(Infraction Infraction)
        {

            InfractionTimers.Add(new Timer(async _ =>
            {
                using (var db = new DatabaseContext())
                {
                    var inf = db.Infractions.FirstOrDefault(x => x.Id == Infraction.Id);
                    if (inf.IsExpired == true) return;
                    var guild = Client.GetGuild(inf.GuildId);
                    var offender = guild.GetUser(inf.OffenderId);

                    var setup = db.GuildSetups.FirstOrDefault(x => x.GuildId == guild.Id);
                    var roles = db.CachedRoles.FirstOrDefault(x => x.UserId == inf.OffenderId);
                    try { db.CachedRoles.Remove(db.CachedRoles.FirstOrDefault(x => x.UserId == inf.OffenderId)); } catch (ArgumentNullException) { return; }
                    inf.IsExpired = true;




                    List<SocketRole> addroles = new List<SocketRole>();
                    foreach (var role in roles.RoleIds) addroles.Add(guild.GetRole(role));

                    await offender.AddRolesAsync(addroles);
                    await offender.RemoveRoleAsync(guild.GetRole(setup.MutedRoleId), new RequestOptions { AuditLogReason = $"Mute expired" });
                    await db.SaveChangesAsync();
                }
            },
            null,
            TimeSpan.FromMilliseconds((DateTime.Now - DateTime.Now.AddMilliseconds(-Infraction.Duration.Value.TotalMilliseconds)).TotalMilliseconds),
            TimeSpan.FromMilliseconds(Timeout.Infinite)));

        }
    }
}
