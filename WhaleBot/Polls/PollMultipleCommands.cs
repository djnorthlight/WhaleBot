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
    public class PollMultipleCommands : ModuleBase
    {
        [Command("poll multi", RunMode = RunMode.Async)]
        [Summary("Creates a multi vote poll\n**Syntax**: `poll Question | Answer 1 | Answer 2` Up to 20 answers")]
        [Priority(3)]
        [RequireUserPermission]
        public async Task PollMultiCommand([Remainder]string input)
        {
            var chan = Context.Channel as IGuildChannel;
            //await chan.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(addReactions: PermValue.Deny));
            List<string> options = new List<string>();
            StringBuilder split = new StringBuilder();

            foreach (char c in input)
            {
                if (c != '|') split.Append(c);
                if (c == '|')
                {
                    options.Add(split.ToString());
                    split.Clear();
                }
            }
            options.Add(split.ToString());
            if (options.Count == 1) return;
            if (options.Count == 2)
            {
                await ReplyAsync("dafuq u trying to do");
                return;
            }
            int count = options.Count - 1;
            if (options.Count >= 21)
            {
                await ReplyAsync("Too many options, max 20");
                return;
            }
            char[] alpha = "abcdefghjiklmnopqrstuvwxyz".ToCharArray();

            var texte = "This is a multi vote poll, all reactions count";
            int id;
            using (var db = new DatabaseContext())
            {
                try { id = db.Polls.OrderByDescending(x => x.Id).First().Id + 1; } catch { id = 0; }
            }


            var msg = await ReplyAsync("", false, new EmbedBuilder { Description = "Please wait..." });

            var poll = new Poll
            {
                MessageId = msg.Id,
                IsMultiple = true,
                Options = options,
                ChannelId = msg.Channel.Id,
                GuildId = (msg.Channel as IGuildChannel).GuildId,
                Votes = new Dictionary<ulong, string>(),
                Emotes = new List<string>()

            };

            using (var db = new DatabaseContext())
            {
                db.Polls.Add(poll);
                await db.SaveChangesAsync();
            }

            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = "Poll #" + poll.Id,
                Description = options.First(),
                Color = new Color(178, 224, 40),
                Footer = new EmbedFooterBuilder { IconUrl = Context.Client.CurrentUser.GetAvatarUrl(), Text = texte }
            };
            options.Remove(options.First());


            int counter = 0;
            foreach (string s in options)
            {
                embed.AddField(new EmbedFieldBuilder
                {
                    IsInline = true,
                    Name = EmojiMaker.Get(alpha[counter]),
                    Value = s,
                });
                counter++;
            }

            embed.WithUrl("http://heeeeeeeey.com/");
            await msg.ModifyAsync(x => x.Embed = embed.Build());

            for (int i = 0; i != count; i++)
            {
                await msg.AddReactionAsync(new Discord.Emoji(EmojiMaker.Get(alpha[i])));
                await Task.Delay(1100);
            }






        }
    }
}
