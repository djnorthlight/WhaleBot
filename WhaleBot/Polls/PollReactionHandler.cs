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
    public class ReactionHandler
    {
        DiscordSocketClient client;
        public ReactionHandler(DiscordSocketClient client)
        {
            this.client = client;
            client.ReactionAdded += Client_ReactionAdded;
            //client.ReactionRemoved += Client_ReactionRemoved;
        }


        //private Task Client_ReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        //{
        //    if (PollCommands.poll == null) return Task.CompletedTask;
        //    if (arg3.MessageId != PollCommands.poll.MessageId || PollCommands.poll.Multiple || arg3.UserId == client.CurrentUser.Id || PollCommands.poll == null) return Task.CompletedTask;
        //    if (Program.votes.ContainsValue(arg3.Emote)) Program.whoAdded.Remove(arg3.User.Value);
        //    return Task.CompletedTask;
        //}

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {

            using(var db = new DatabaseContext())
            {
                var currentPoll = db.Polls.FirstOrDefault(x => x.MessageId == arg3.MessageId);
                if (currentPoll == null) return;
                if (arg3.MessageId != currentPoll.MessageId || currentPoll.IsMultiple) return;
                var msg = await arg1.GetOrDownloadAsync();
                if (db.Polls.Find(currentPoll.Id).Votes == null) currentPoll.Votes = new Dictionary<ulong, string>();
                if (db.Polls.Find(currentPoll.Id).Emotes == null) currentPoll.Emotes = new List<string>();
                if (db.Polls.Find(currentPoll.Id).Options == null) currentPoll.Options = new List<string>();


                var message = await arg2.GetMessageAsync(currentPoll.MessageId);

                if (message.Embeds.First().Fields.ToDictionary(x => x.Name).ContainsKey(arg3.Emote.Name) || arg3.User.Value.Id == client.CurrentUser.Id)
                {
                    if (arg3.User.Value.Id == client.CurrentUser.Id)
                    {
                        db.Polls.Find(currentPoll.Id).Emotes.Add(arg3.Emote.Name);
                    }
                    else
                    {
                        try { db.Polls.Find(currentPoll.Id).Votes.Remove(arg3.User.Value.Id); } catch { }
                        db.Polls.Find(currentPoll.Id).Votes.Add(arg3.User.Value.Id, arg3.Emote.Name);
                    }

                }
                else
                {
                    await arg3.Message.Value.RemoveReactionAsync(arg3.Emote, arg3.User.Value);
                }
                await db.SaveChangesAsync(true);
            }



        }
    }
}
