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
    public class GiveawayHandlers
    {
        DiscordSocketClient client;
        public GiveawayHandlers(DiscordSocketClient client)
        {
            this.client = client;
            client.ReactionAdded += Client_ReactionAdded;
            client.ReactionRemoved += Client_ReactionRemoved;
        }

        private Task Client_ReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            using (var db = new DatabaseContext())
            {
                if (!db.Giveaways.Any(x => x.MessageId == arg1.Id)) return Task.CompletedTask;
                db.Giveaways.FirstOrDefault(x => x.MessageId == arg1.Id).UserIds.Remove(arg3.UserId);
                db.SaveChanges();
            }
            return Task.CompletedTask;
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (arg3.UserId == client.CurrentUser.Id) return;
            var message = await arg1.GetOrDownloadAsync();
            using (var db = new DatabaseContext())
            {
                if (!db.Giveaways.Any(x => x.MessageId == arg1.Id)) return;
                if (arg3.Emote.ToString() != "<:whalebot:361164367506571264>") { await message.RemoveReactionAsync(arg3.Emote, client.GetUser(arg3.UserId)); return; }
                db.Giveaways.FirstOrDefault(x => x.MessageId == arg1.Id).UserIds.Add(arg3.UserId);
                db.SaveChanges();
            }           
        }
    }
}
