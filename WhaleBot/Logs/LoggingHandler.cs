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
    public class LoggingHandler
    {
        private DiscordSocketClient client;
        private DateTime lastMessage = DateTime.Now;
        public LoggingHandler(DiscordSocketClient client)
        {
            this.client = client;
            client.MessageDeleted += Client_MessageDeleted;
            client.MessageUpdated += Client_MessageUpdated;
            client.MessageReceived += Client_MessageReceived;
        }

        private Task Client_MessageReceived(SocketMessage arg)
        {
            using (var db = new DatabaseContext())
            {
                if (arg == null) return Task.CompletedTask;
                if (arg.Channel.GetType() == typeof(SocketDMChannel)) return Task.CompletedTask;

                db.LoggedMessages.Add(new LoggedMessage
                {
                    GuildId = (arg.Channel as SocketGuildChannel).Guild.Id,
                    ChannelId = arg.Channel.Id,
                    AuthorId = arg.Author.Id,
                    MessageId = arg.Id,
                    Content = arg.Content,
                    Timestamp = arg.Timestamp.DateTime
                });
                db.SaveChanges();
            }
            return Task.CompletedTask;
        }

        private async Task Client_MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            if (arg3 == null) return;
            if (arg2.EditedTimestamp == null) return;
            if (arg3.GetType() == typeof(SocketDMChannel)) return;
            var oldmess = await arg1.GetOrDownloadAsync();
            GuildLoggingSetup setup;
            using (var db = new DatabaseContext())
            {
                setup = db.GuildLoggingSetups.FirstOrDefault(x => x.GuildId == (arg3 as SocketGuildChannel).Guild.Id);
                var message = db.LoggedMessages.FirstOrDefault(x => x.MessageId == oldmess.Id);
                message.IsEdited = true;
                message.Edits.Add(arg2.Content, arg2.EditedTimestamp.Value.DateTime);
                db.SaveChanges();
                if (db.GuildStarringSetups.FirstOrDefault(x => x.GuildId == (arg3 as SocketGuildChannel).Guild.Id).StarboardChannelId == arg3.Id) return;
            }

            if (setup?.EditChannelId == 0) return;
            if (arg2.Content == "") return;

            await (arg3 as SocketGuildChannel).Guild.GetTextChannel(setup.EditChannelId).SendMessageAsync("", false, new EmbedBuilder
            {
                Title = $"A message has been edited in {arg3.Name}!",
                Fields = new List<EmbedFieldBuilder> { new EmbedFieldBuilder { Name = "Before:", Value = $"```{oldmess.Content}```" }, { new EmbedFieldBuilder { Name = "After:", Value = $"```{arg2.Content}```" } } },
                Footer = new EmbedFooterBuilder { Text = $"Author: {arg2.Author.Username}", IconUrl = arg2.Author.GetAvatarUrl() },
                ThumbnailUrl = arg2.Author.GetAvatarUrl(),
                Color = new Color(178, 224, 40),
                Timestamp = arg2.EditedTimestamp
            }.WithUrl("http://heeeeeeeey.com/"));
        }

        private async Task Client_MessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            var arg = await arg1.GetOrDownloadAsync();
            LoggedMessage delmess;

            GuildLoggingSetup setup;
            using (var db = new DatabaseContext())
            {
                setup = db.GuildLoggingSetups.FirstOrDefault(x => x.GuildId == (arg2 as SocketGuildChannel).Guild.Id);
                var message = db.LoggedMessages.FirstOrDefault(x => x.MessageId == arg1.Id);
                delmess = message;
                if (message != null)
                {
                    message.IsDeleted = true;
                    db.SaveChanges();
                }
            }
            if (setup?.RemoveChannelId == 0) return;
            bool noAttachments = true;
            if (arg?.Attachments?.Count != null) if (arg.Attachments.Count != 0) noAttachments = false;
            if (arg == null && noAttachments && string.IsNullOrEmpty(delmess.Content)) return;

            if (DateTime.Now.AddMilliseconds(-500) > lastMessage && delmess.ChannelId != 324321707777196034)
            {
                List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
                if ((arg?.Content?.Length ?? delmess.Content.Length) > 0) fields.Add(new EmbedFieldBuilder { Name = "Content:", Value = $"```{arg?.Content ?? delmess.Content}```" });
                if(!noAttachments) if (arg.Attachments.Count == 1) fields.Add(new EmbedFieldBuilder { Name = "Attachments:", Value = $"```{arg.Attachments.First().Filename}```" });
                if (fields.Count == 0) return;

                SocketUser user = null;
                SocketGuildChannel chan = null;
                if (arg == null) chan = (client.GetGuild(delmess.GuildId)).GetChannel(delmess.ChannelId);
                if (arg == null) user = chan.GetUser(delmess.AuthorId);
                if (arg.Content == "?cleanup") return;

                await (arg2 as SocketGuildChannel).Guild.GetTextChannel(setup.RemoveChannelId).SendMessageAsync("", false, new EmbedBuilder
                {
                    Title = $"A message has been deleted in {arg?.Channel?.Name ?? chan.Name}!",
                    Fields = fields,
                    Footer = new EmbedFooterBuilder { Text = $"Author: {arg?.Author?.Username ?? user.Username}", IconUrl = arg?.Author?.GetAvatarUrl() ?? user.GetAvatarUrl() },
                    Color = new Color(178, 224, 40),
                    ThumbnailUrl = arg?.Author?.GetAvatarUrl() ?? user.GetAvatarUrl(),
                    Timestamp = arg?.Timestamp ?? delmess.Timestamp,

                }.WithUrl("http://heeeeeeeey.com/"));
            }
            lastMessage = DateTime.Now;

        }
    }
}
