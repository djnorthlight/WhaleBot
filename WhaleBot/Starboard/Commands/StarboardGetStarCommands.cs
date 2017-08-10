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
    public class StarboardGetStarCommands : ModuleBase<SocketCommandContext>
    {
        [Command("star")]
        [Summary("Recall a message from the starboard by ID")]
        [RequireUserPermission]
        public async Task StarboardStarCommand(int id)
        {
            //the coconut nut is a giant nut
            using(var db = new DatabaseContext())
            {
                var mess = db.StarredMessages.FirstOrDefault(x => x.Id == id);
                if(mess == null)
                {
                    await ReplyAsync($"A star with id {id} doesnt exist");
                    return;
                }
                if(mess.GuildId != Context.Guild.Id)
                {
                    await ReplyAsync($"That star isn't from this guild");
                    return;
                }
                var user = Context.Guild.GetUser(mess.AuthorId);
                var chan = Context.Guild.GetTextChannel(mess.ChannelId);
                var mess2 = await chan.GetMessageAsync(mess.MessageId);

                var embed = new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder { Name = user.Nickname ?? user.Username, IconUrl = user.GetAvatarUrl() },
                    Description = mess2.Content,
                    Color = new Color(178, 224, 40),
                    Footer = new EmbedFooterBuilder { Text = $"ID: {mess.Id}" },
                    Timestamp = mess2.Timestamp
                };
                if (mess2?.Attachments.Count != 0) embed.ImageUrl = mess2.Attachments.First().Url;
                await ReplyAsync("", false, embed);
            }



        }
    }
}
