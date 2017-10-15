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
    public class InfractionsCommands : ModuleBase<SocketCommandContext>
    {
        [Command("infractions")][Alias("inf", "search")][RequireUserPermission][Summary("Displays infractions of a given user")]
        public async Task InfractionsCommand([Remainder]SocketGuildUser userr = null)
        {
            var user = userr ?? Context.User;


            using (var db = new DatabaseContext())
            {
                var UserInfractions = db.Infractions.Where(x => x.OffenderId == user.Id && x.GuildId == Context.Guild.Id);

                if (UserInfractions.Count() == 0)
                {
                    await ReplyAsync($"{user.Username} doesn't have any infractions");
                    return;
                }

                var pages = new List<EmbedBuilder>();

                int currentPage = -1;

                foreach (var Infraction in UserInfractions)
                {
                    if (currentPage == -1 || pages[currentPage].Fields.Count() >= 25)
                    {
                        currentPage++;
                        pages.Add(new EmbedBuilder
                        {
                            Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                            Title = $"{user.Username}'s infractions " + (currentPage == 0 ? "" : (currentPage + 1).ToString()),
                            Color = new Color(178, 224, 40),
                        }.WithUrl("http://heeeeeeeey.com/"));
                    }

                    pages[currentPage].AddField(new EmbedFieldBuilder { Name = Infraction.Type.ToString(), Value = Infraction.Reason, IsInline = true });
                }

                foreach (var page in pages) await ReplyAsync("", false, page);
            }


        }
    }
}
