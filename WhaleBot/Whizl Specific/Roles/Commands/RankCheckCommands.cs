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
    public class RankCheckCommands : ModuleBase<SocketCommandContext>
    {

        [Command("rank check")]
        [WhizlSpecific]
        [RequireUserPermission]
        public async Task RankCheckCommand()
        {
            await Context.Guild.DownloadUsersAsync();

            var rankedUsers = Context.Guild.Users.Cast<IGuildUser>().Where(x => x.RoleIds.Contains<ulong>(347140287275073546));

            foreach(var rankedUser in rankedUsers)
            {
                var gAuthor = rankedUser as SocketGuildUser;
                var gChannel = Context.Channel as SocketGuildChannel;

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
                        if (db.LoggedMessages.Where(x => x.AuthorId == gAuthor.Id && x.Timestamp > DateTime.Now.AddDays(-(counter * 7))).Count() == 0)
                            if (gAuthor.Roles.Any(x => x.Id == role.Id))
                            {
                                if (role.Id == (ulong)RoleLevel.Recognised) await gAuthor.RemoveRoleAsync(gChannel.Guild.GetRole(347140287275073546));
                                await gAuthor.RemoveRoleAsync(gChannel.Guild.GetRole(role.Id), new RequestOptions { AuditLogReason = "Was inactive for 7 days" });
                                db.LoggedMessages.Add(new LoggedMessage
                                {
                                    GuildId = 0,
                                    ChannelId = 0,
                                    AuthorId = gAuthor.Id,
                                    MessageId = 0,
                                    Content = "",
                                    Timestamp = DateTime.Now
                                });
                                db.SaveChanges();
                            }
                        counter++;
                    }
                }
            }

            await ReplyAsync("👍");
        }
    }
}
