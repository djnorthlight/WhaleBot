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
    public class BlacklistedEmojiHandler : ModuleBase<SocketCommandContext>
    {
        DiscordSocketClient client;
        public BlacklistedEmojiHandler(DiscordSocketClient client)
        {
            this.client = client;
            client.MessageReceived += Client_MessageReceived;
            client.ReactionAdded += Client_ReactionAdded;
            client.MessageUpdated += Client_MessageUpdated;
        }

        private async Task Client_MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            await CheckMessage(arg2);
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            var guild = (arg3.Channel as SocketGuildChannel).Guild;
            if (guild.Id == 324282875035779072 && arg3.Emote.Name.Contains("GW") && guild.CurrentUser.Nickname == "WhalyBot")
            {
                await arg3.Message.Value.RemoveReactionAsync(arg3.Emote, arg3.User.Value);
            }
        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            await CheckMessage(arg);
        }

        private async Task CheckMessage(SocketMessage mess)
        {
            var guild = (mess.Channel as SocketGuildChannel).Guild;
            if (guild.Id == 324282875035779072 && mess.ToString().ToLower().Contains("<:gw") && guild.CurrentUser.Nickname == "WhalyBot")
            {
                await mess.DeleteAsync();
            }
        }
    }
}
