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
    class GoogleReactionHandler
    {
        private DiscordSocketClient client;
        private GoogleEmbeds embeds;

        public GoogleReactionHandler(DiscordSocketClient client, GoogleEmbeds embeds)
        {
            this.client = client;
            this.embeds = embeds;
            client.ReactionAdded += Client_ReactionAdded;
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            var currentEmbed = embeds.List.FirstOrDefault(x => x.MessageId == arg3.MessageId);
            if (currentEmbed == null) return;
            if (arg3.UserId == 332106465412251648) return;

            var msg = await arg2.GetMessageAsync(currentEmbed.MessageId) as SocketUserMessage;

            if (arg3.Emote.Name == "◀")
            {
                if(currentEmbed.CurrentPage != 1)await msg.ModifyAsync(x => x.Embed = currentEmbed.GetEmbed(false));
                await arg3.Message.Value.RemoveReactionAsync(arg3.Emote, arg3.User.Value);
            }
            else if (arg3.Emote.Name == "▶")
            {
                await msg.ModifyAsync(x => x.Embed = currentEmbed.GetEmbed(true));
                await arg3.Message.Value.RemoveReactionAsync(arg3.Emote, arg3.User.Value);
            }
            else { await arg3.Message.Value.RemoveReactionAsync(arg3.Emote, arg3.User.Value); }

        }
    }
}
