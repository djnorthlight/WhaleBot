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
    public class DeleteLogGettingCommands : ModuleBase<SocketCommandContext>
    {
        [Command("deleted")]
        [RequireUserPermission]
        [Summary("Returns a file with a number of deleted messages\n**Syntax:** `deleted @user/#channel number` User or channel are optional")]
        public async Task DeletedCommand(int number)
        {
            using (var db = new DatabaseContext())
            {
                var currentLogs = db.LoggedMessages.Where(x => x.GuildId == Context.Guild.Id && x.IsDeleted).OrderByDescending(x => x.Timestamp);


                int counter = 0;
                using (StreamWriter file = new StreamWriter(File.OpenWrite("log.txt")))
                {
                    foreach (var log in currentLogs)
                    {
                        if (counter == number) break;
                        file.WriteLine($"[{log.Timestamp}] #{Context.Guild.GetChannel(log.ChannelId).Name} {Context.Guild.GetUser(log.AuthorId).Username}: {log.Content}");
                        counter++;
                    }
                }


                await Context.Channel.SendFileAsync("log.txt");
                File.Delete("log.txt");

            }
        }
        [Command("deleted")]
        [RequireUserPermission]
        [Remarks("Exclude from help")]
        public async Task DeletedCommand(SocketTextChannel channel, int number)
        {
            using (var db = new DatabaseContext())
            {
                var currentLogs = db.LoggedMessages.Where(x => x.GuildId == Context.Guild.Id && x.ChannelId == channel.Id && x.IsDeleted).OrderByDescending(x => x.Timestamp);


                int counter = 0;
                using (StreamWriter file = new StreamWriter(File.OpenWrite("log.txt")))
                {
                    foreach (var log in currentLogs)
                    {
                        if (counter == number) break;
                        file.WriteLine($"[{log.Timestamp}] #{Context.Guild.GetChannel(log.ChannelId).Name} {Context.Guild.GetUser(log.AuthorId).Username}: {log.Content}");
                        counter++;
                    }
                }


                await Context.Channel.SendFileAsync("log.txt");
                File.Delete("log.txt");

            }
        }
        [Command("deleted")]
        [RequireUserPermission]
        [Remarks("Exclude from help")]
        public async Task DeletedCommand(SocketGuildUser user, int number)
        {
            using (var db = new DatabaseContext())
            {
                var currentLogs = db.LoggedMessages.Where(x => x.GuildId == Context.Guild.Id && x.AuthorId == user.Id && x.IsDeleted).OrderByDescending(x => x.Timestamp);


                int counter = 0;
                using (StreamWriter file = new StreamWriter(File.OpenWrite("log.txt")))
                {
                    foreach (var log in currentLogs)
                    {
                        if (counter == number) break;
                        file.WriteLine($"[{log.Timestamp}] #{Context.Guild.GetChannel(log.ChannelId).Name} {Context.Guild.GetUser(log.AuthorId).Username}: {log.Content}");
                        counter++;
                    }
                }


                await Context.Channel.SendFileAsync("log.txt");
                File.Delete("log.txt");

            }
        }
    }
}
