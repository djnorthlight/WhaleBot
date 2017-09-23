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
    public class GiveawayCommands : ModuleBase<SocketCommandContext>
    {
        [Command("giveaway")][RequireUserPermission]
        public async Task GiveawayCommand([Remainder]string content)
        {
            var mess = await ReplyAsync("", false, new EmbedBuilder { Description = "Please wait..." });
            int Id;

            using(var db = new DatabaseContext())
            {
                db.Giveaways.Add(new Giveaway { MessageId = mess.Id, GuildId = Context.Guild.Id });
                db.SaveChanges();
                Id = db.Giveaways.FirstOrDefault(x => x.MessageId == mess.Id).Id;
            }

            await mess.ModifyAsync(x => x.Embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = $"Giveaway of {content}",
                Color = new Color(178, 224, 40),
                Description = "To enter this giveaway react with <:whalebot:361164367506571264>",
                Footer = new EmbedFooterBuilder { Text=$"ID: {Id}" }
            }.WithUrl("http://heeeeeeeey.com/").Build());
        
            await mess.AddReactionAsync(Emote.Parse("<:whalebot:361164367506571264>"));

        }

        [Command("giveaway close")][RequireUserPermission]
        public async Task GiveawayCloseCommand(int id)
        {
            Giveaway giveaway;
            using (var db = new DatabaseContext())
            {
                giveaway = db.Giveaways.FirstOrDefault(x => x.Id == id);
                try { db.Giveaways.Remove(giveaway); } catch { }
                db.SaveChanges();
            }

            if (giveaway == null)
            {
                await ReplyAsync("A giveaway with that ID doesnt exist");
                return;
            }
            if (giveaway.GuildId != Context.Guild.Id)
            {
                await ReplyAsync("That giveaway isn't from this guild");
                return;
            }

            ulong winner;
            try { winner = giveaway.UserIds.ToArray()[new Random().Next(giveaway.UserIds.Count)]; }
            catch(IndexOutOfRangeException) { await ReplyAsync("Giveaway closed, nobody participated"); return; }

            await ReplyAsync("", false, new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = "Giveaway closed",
                Color = new Color(178, 224, 40),
                Description = $"The winner is <@{winner}>"
            }.WithUrl("http://heeeeeeeey.com/"));

            var tag = await ReplyAsync($"<@{winner}>");
            await tag.DeleteAsync();
        }
    }
}
