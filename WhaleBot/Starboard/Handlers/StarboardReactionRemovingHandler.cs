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
    public class StarboardReactionRemovingHandler
    {
        DiscordSocketClient client;
        public StarboardReactionRemovingHandler(DiscordSocketClient client)
        {
            this.client = client;
            client.ReactionRemoved += Client_ReactionRemoved;
        }

        private async Task Client_ReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (arg3.Emote.Name != "⭐") return;
            using (var db = new DatabaseContext())
            {
                var smessages = db.StarredMessages.Where(x => x.GuildId == (arg2 as SocketGuildChannel).Guild.Id);
                if(smessages.Any(x => x.IsPinned && (x.MessageId == arg3.MessageId || x.StarboardMessageId == arg3.MessageId)))
                {
                    var setup = db.GuildStarringSetups.FirstOrDefault(x => x.GuildId == (arg2 as SocketGuildChannel).Guild.Id);
                    var pin = smessages.FirstOrDefault(x => x.MessageId == arg3.MessageId || x.StarboardMessageId == arg3.MessageId);
                    var starboard = client.GetGuild((arg2 as SocketGuildChannel).Guild.Id).GetTextChannel(setup.StarboardChannelId) as SocketTextChannel;

                    db.StarredMessages.FirstOrDefault(x => x.Id == pin.Id).Stars--;
                    db.StarredMessages.FirstOrDefault(x => x.Id == pin.Id).StarredByIds.Remove(arg3.UserId);

                    var pinmess = await starboard.GetMessageAsync(pin.StarboardMessageId);
                    await (pinmess as SocketUserMessage).ModifyAsync(x => x.Content = $"<#{pin.ChannelId}> ⭐{pin.Stars}");
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
