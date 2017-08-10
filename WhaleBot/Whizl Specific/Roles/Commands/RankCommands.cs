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
    public class RankCommands : ModuleBase<SocketCommandContext>
    {
        [Command("rank")][WhizlSpecific][RequireUserPermission]
        public async Task RankCommand(IGuildUser user = null)
        {
            using (var db = new DatabaseContext())
            {
                var info = db.MemberRoleInfos.FirstOrDefault(x => x.UserId == Context.User.Id).DaysActive;
                int days = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (info < 3) days = info - 3;
                    if (info < 5) days = info - 5;
                    if (info < 7) days = info - 7;
                    if (info < 14) days = info - 14;
                }

                await ReplyAsync("", false, new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                    Title = "Rank info!",
                    Fields = new List<EmbedFieldBuilder> {new EmbedFieldBuilder { IsInline = true, Name = "Active days", Value = db.MemberRoleInfos.FirstOrDefault(x => x.UserId == Context.User.Id)},
                    new EmbedFieldBuilder{IsInline = true, Name = "Days to next rank", Value = days.ToString() } }
                });
                
            }
        }
    }
}
