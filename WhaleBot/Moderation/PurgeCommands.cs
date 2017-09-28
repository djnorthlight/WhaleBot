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
    public class PurgeCommands : ModuleBase<SocketCommandContext>
    {
        [Command("purge")]
        [Remarks("Exclude from help")]
        [RequireUserPermission]
        public async Task PurgeCommand(int number)
        {
            var mess = await Context.Channel.GetMessagesAsync(number + 1).Flatten();
            await Context.Channel.DeleteMessagesAsync(mess);
            var reply = await ReplyAsync("", false, new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = "Deleted messages!",
                Color = new Color(178, 224, 40),
                Description = $"Deleted {number} messages"
            });

            var t = Task.Run(async () =>
            {
                await Task.Delay(10000);
                await reply.DeleteAsync();
            });
        }
        [Command("purge")]
        [Remarks("Exclude from help")]
        [RequireUserPermission]
        public async Task PurgeCommand(SocketGuildUser user, int number)
        {
            await Context.Message.DeleteAsync();
            var mess = await Context.Channel.GetMessagesAsync(100).Flatten();
            await Context.Channel.DeleteMessagesAsync(mess.Where(x => x.Author == user).OrderByDescending(x => x.Timestamp).Take(number));
            var reply = await ReplyAsync("", false, new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = "Deleted messages!",
                Color = new Color(178, 224, 40),
                Description = $"Deleted {number} messages from {user.Mention}"
            });

            var t = Task.Run(async () =>
            {
                await Task.Delay(10000);
                await reply.DeleteAsync();
            });
        }

        [Command("purge")]
        [Remarks("Exclude from help")]
        [RequireUserPermission]
        public async Task PurgeCommand(int number, [Remainder]SocketGuildUser user)
        {
            await PurgeCommand(user, number);
        }
    }
}
