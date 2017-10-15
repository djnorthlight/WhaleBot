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
    public class PollCloseCommands : ModuleBase<SocketCommandContext>
    {
        [Command("poll close")]
        [Summary("Closes a poll\n**Syntax:** `poll close Id`")]
        [RequireUserPermission]
        [Priority(2)]
        public async Task PollCloseCommand(int id)
        {
            Poll currentPoll = null;
            using ( var db = new DatabaseContext())
            {
                try { currentPoll = db.Polls.FirstOrDefault(x => x.Id == id); } catch { }              
            }
                
            if (currentPoll == null)
            {
                await ReplyAsync("A poll with that id doesnt exist");
                return;
            }
            if(currentPoll.GuildId != Context.Guild.Id)
            {
                await ReplyAsync("That poll is from a different guild");
                return;
            }
            var channel = (Context.Guild.GetChannel(currentPoll.ChannelId) as ISocketMessageChannel);
            IMessage message = await channel.GetMessageAsync(currentPoll.MessageId);


            char[] alpha = "abcdefghjiklmnopqrstuvwxyz".ToCharArray();
            Dictionary<string, int> pepe = new Dictionary<string, int>();
            pepe.Clear();


            

            foreach (var c in currentPoll.Emotes)
            {
                try { pepe.Add(c.ToString(), 0); } catch { }          
            }
            if(!currentPoll.IsMultiple)
            foreach (var c in currentPoll.Votes.Values)
            {
                pepe.TryGetValue(c.ToString(), out int val);
                val++;
                pepe.Remove(c.ToString());
                pepe.Add(c.ToString(), val);
                }
            else
            {
                var cyka = await Context.Channel.GetMessageAsync(currentPoll.MessageId) as IUserMessage;
                foreach (var c in cyka.Reactions)
                {
                    pepe.Add(c.Key.Name, c.Value.ReactionCount - 1);
                }
            }


            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = $"Poll #{currentPoll.Id} results",
                Description = message.Embeds.First().Description,
                Color = new Color(178, 224, 40),
            };

           

            for (int i = 0; i<pepe.Count; i++)
            {
                var emoji = EmojiMaker.Get(alpha[i]);
                EmbedField field = message.Embeds.First().Fields.Where(x => x.Name == emoji).First();
               
                pepe.TryGetValue(emoji, out int value);

                embed.AddInlineField(EmojiMaker.Get(alpha[i]) + field.Value, $"Total votes: {value}");
            }

            using(var db = new DatabaseContext())
            {
                db.Polls.Remove(currentPoll);
                await db.SaveChangesAsync();
            }


            embed.WithUrl("http://heeeeeeeey.com/");
            await ReplyAsync("", false, embed);


        }
    }
}
