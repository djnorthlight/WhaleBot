using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using System.IO;

namespace WhaleBot
{
    public class GoogleCommands : ModuleBase<SocketCommandContext>
    {
        private GoogleEmbeds Embeds;
        public GoogleCommands(GoogleEmbeds Embeds)
        {
            this.Embeds = Embeds;
        }

        [Command("google")][Alias("g")][Summary("Googles something")]
        [RequireUserPermission]
        public async Task GoogleCommand([Remainder]string query)
        {
            var cyka = (new GoogleEmbed
            {
                LastIndex = 1,
                Query = query,
                ResultsPerPage = 5,
                Pages = new Dictionary<int, List<Google.Apis.Customsearch.v1.Data.Result>>()
            });

            var msg = await ReplyAsync("", false, cyka.GetEmbed(true));
            cyka.MessageId = msg.Id;
            await msg.AddReactionAsync(new Emoji("◀"));
            await Task.Delay(250);
            await msg.AddReactionAsync(new Emoji("▶"));
            Embeds.List.Add(cyka);
        }


        //[Command("google next")][Remarks("Exclude from help")]
        //public async Task GoogleNextCommand(ulong Id)
        //{
        //    var embed = Embeds.List.FirstOrDefault(x => x.MessageId == Id);
        //    var msg = await Context.Channel.GetMessageAsync(embed.MessageId) as SocketUserMessage;
        //    await msg.ModifyAsync(x => x.Embed = embed.GetEmbed());
        //}

        //[Command("google short")][Alias("gs")][Summary("Googles something returning only one result")]
        //[RequireUserPermission]
        //public async Task GooogleShortCommand([Remainder]string query)
        //{
        //    var listRequest = Search.Cse.List(query);
        //    listRequest.Cx = SearchEngineId;
        //    listRequest.Start = 1;
        //    var search = (await listRequest.ExecuteAsync()).Items;
        //    string reply = $"[{search.First().Title}]({search.First().Link})";
        //    await ReplyAsync("", false, new EmbedBuilder
        //    {
        //        Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
        //        Description = reply,
        //        Title = "Google results",
        //        Color = new Color(178, 224, 40),
        //    }.WithUrl("http://heeeeeeeey.com/"));
        //}
    }
}
