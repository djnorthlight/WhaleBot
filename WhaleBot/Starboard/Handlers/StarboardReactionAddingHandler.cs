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
    public class StarboardReactionAddingHandler
    {
        private DiscordSocketClient client;
        public StarboardReactionAddingHandler(DiscordSocketClient client)
        {
            this.client = client;
            client.ReactionAdded += Client_ReactionAdded;
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (arg3.Emote.Name != "⭐") return;
            using(var db = new DatabaseContext())
            {
                var logmess = db.LoggedMessages.FirstOrDefault(x => x.MessageId == arg1.Id);
                var mess = db.StarredMessages.FirstOrDefault(x => x.MessageId == arg1.Id || x.StarboardMessageId == arg1.Id);
                StarredMessage smess = null;
                if(mess != null) if (mess.StarredByIds.Contains(arg3.UserId)) return;
                if (mess == null)
                {
                    smess = new StarredMessage
                    {
                        GuildId = (arg2 as SocketGuildChannel).Guild.Id,
                        ChannelId = arg2.Id,
                        AuthorId = logmess.AuthorId,
                        MessageId = logmess.MessageId,
                        Stars = 1,
                        Timestamp = logmess.Timestamp
                    };
                    db.StarredMessages.Add(smess);
                  
                }
                if (smess != null) mess = smess;
                else mess.Stars++;
                mess.StarredByIds.Add(arg3.UserId);
                var guildid = (arg2 as SocketGuildChannel).Guild.Id;
                var setup = db.GuildStarringSetups.FirstOrDefault(x => x.GuildId == guildid);
                if (mess.Stars == setup.StarsRequired && !mess.IsPinned)
                {
                    var chan = client.GetGuild(guildid).GetTextChannel(setup.StarboardChannelId);
                    var user = chan.GetUser(mess.AuthorId);
                    var mess2 = await arg2.GetMessageAsync(mess.MessageId);
                    var embed = new EmbedBuilder
                    {
                        Author = new EmbedAuthorBuilder { Name = user.Nickname ?? user.Username, IconUrl = user.GetAvatarUrl() },
                        Description = mess2.Content,
                        Color = new Color(178, 224, 40),
                        Footer = new EmbedFooterBuilder { Text = $"ID: {mess.Id}" },
                        Timestamp = mess2.Timestamp
                    };
                    if (mess2?.Attachments.Count != 0) embed.ImageUrl = mess2.Attachments.First().Url;
                    if (mess2?.Embeds.Count != 0 && mess2?.Embeds?.First()?.Url != null) embed.ImageUrl = mess2.Embeds.First().Url;

                    var starmess = await chan.SendMessageAsync($"<#{arg2.Id}> ⭐{mess.Stars}", false, embed);
                    mess.StarboardMessageId = starmess.Id;
                    mess.IsPinned = true;

                } else if (mess.IsPinned)
                {
                    var chan = (arg2 as SocketGuildChannel).Guild.GetTextChannel(mess.ChannelId);
                    var starmess = await (arg2 as SocketGuildChannel).Guild.GetTextChannel(setup.StarboardChannelId).GetMessageAsync(mess.StarboardMessageId);
                    await (starmess as SocketUserMessage).ModifyAsync(x => x.Content = $"{chan.Mention} ⭐{mess.Stars}");
                }
                await db.SaveChangesAsync();
            }
        }
    }
}
