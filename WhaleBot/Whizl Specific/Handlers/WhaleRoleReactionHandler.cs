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
    public class WhaleRoleReactionHandler
    {
        DiscordSocketClient client;
        public WhaleRoleReactionHandler(DiscordSocketClient client)
        {
            this.client = client;
            client.ReactionAdded += ReactionHandler;
        }


        private async Task ReactionHandler(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            var msg = await arg1.GetOrDownloadAsync();
            if (arg3.MessageId != WhaleRoleMessageHandler.LastMessageId) return;
            if (arg3.User.Value.IsBot) return;
            if (!(arg3.User.Value as IGuildUser).RoleIds.Contains<ulong>(338109400411537408))
            {
                await (arg3.User.Value as SocketGuildUser).AddRoleAsync((msg.Channel as IGuildChannel).Guild.GetRole(338109400411537408), new RequestOptions {AuditLogReason = "Captured the whale!" });
                var msg2 = await client.GetGuild(324282875035779072).GetTextChannel(324284774527139840).GetMessageAsync(WhaleRoleMessageHandler.LastMessageId);
                using (var db = new DatabaseContext())
                {
                    db.RoleAssignments.Add(new RoleAssignment { HourGiven = DateTime.Now, UserId = arg3.UserId });
                    var whalehunter = db.WhaleHunterCounts.FirstOrDefault(x => x.UserId == arg3.UserId);
                    if (whalehunter == null)
                    {
                        whalehunter = new WhaleHunterCount { UserId = arg3.UserId, WhaleCount = 0 };
                        db.WhaleHunterCounts.Add(whalehunter);
                    }
                    whalehunter.WhaleCount++;
                    db.SaveChanges();
                }
                await msg2.DeleteAsync();
            }
            else
            {
                await msg.RemoveReactionAsync(arg3.Emote, arg3.User.Value);
            }

        }
    }
}
