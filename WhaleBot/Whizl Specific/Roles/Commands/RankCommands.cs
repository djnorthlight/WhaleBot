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
        public async Task RankCommand(SocketUser user = null)
        {
            using (var db = new DatabaseContext())
            {
                user = user ?? Context.User;
                var info = db.MemberRoleInfos.FirstOrDefault(x => x.UserId == user.Id).DaysActive;
                int days = 0;

                if (info < 3) days = 3 - info;
                else if (info < 5) days = 5 - info;
                else if (info < 7) days = 7 - info;
                else if (info < 14) days = 14 - info;


                await ReplyAsync("", false, new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                    Title = $"{user.Username}'s rank info!",
                    Color = new Color(178, 224, 40),
                    Fields = new List<EmbedFieldBuilder> {new EmbedFieldBuilder { IsInline = true, Name = "Active days", Value = db.MemberRoleInfos.FirstOrDefault(x => x.UserId == user.Id).DaysActive},
                    new EmbedFieldBuilder{IsInline = true, Name = "Days to next rank", Value = days.ToString() } }
                }.WithUrl("http://heeeeeeeey.com/"));
                
            }
        }
    }
}
