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
    public class EditLogGettingCommands : ModuleBase<SocketCommandContext>
    {
        [Command("edited")]
        [RequireUserPermission]
        [Summary("Returns a file with a number of edited messages\n**Syntax:** `edited @user/#channel number` User or channel are optional")]
        public async Task EditedCommand(int number)
        {
            using (var db = new DatabaseContext())
            {
                var currentLogs = db.LoggedMessages.Where(x => x.GuildId == Context.Guild.Id && x.IsEdited).OrderByDescending(x => x.Timestamp);


                int counter = 0;
                using (StreamWriter file = new StreamWriter(File.OpenWrite("log.txt")))
                {
                    foreach (var log in currentLogs)
                    {
                        if (counter == number) break;
                        file.WriteLine($"[{log.Timestamp}] #{Context.Guild.GetChannel(log.ChannelId).Name} {Context.Guild.GetUser(log.AuthorId).Username}: {log.Content}");
                        foreach (var edit in log.Edits) file.WriteLine($"[{edit.Value}] {edit.Key}");
                        counter++;
                    }
                }


                await Context.Channel.SendFileAsync("log.txt");
                File.Delete("log.txt");

            }
        }
        [Command("edited")]
        [RequireUserPermission]
        [Remarks("Exclude from help")]
        public async Task EditedCommand(SocketTextChannel channel, int number)
        {
            using (var db = new DatabaseContext())
            {
                var currentLogs = db.LoggedMessages.Where(x => x.GuildId == Context.Guild.Id && x.ChannelId == channel.Id && x.IsEdited).OrderByDescending(x => x.Timestamp);


                int counter = 0;
                using (StreamWriter file = new StreamWriter(File.OpenWrite("log.txt")))
                {
                    foreach (var log in currentLogs)
                    {
                        if (counter == number) break;
                        file.WriteLine($"[{log.Timestamp}] #{Context.Guild.GetChannel(log.ChannelId).Name} {Context.Guild.GetUser(log.AuthorId).Username}: {log.Content}");
                        foreach (var edit in log.Edits) file.WriteLine($"[{edit.Value}] {edit.Key}");
                        counter++;
                    }
                }


                await Context.Channel.SendFileAsync("log.txt");
                File.Delete("log.txt");

            }
        }
        [Command("edited")]
        [RequireUserPermission]
        [Remarks("Exclude from help")]
        public async Task EditedCommand(SocketGuildUser user, int number)
        {
            using (var db = new DatabaseContext())
            {
                var currentLogs = db.LoggedMessages.Where(x => x.GuildId == Context.Guild.Id && x.AuthorId == user.Id && x.IsEdited).OrderByDescending(x => x.Timestamp);


                int counter = 0;
                using (StreamWriter file = new StreamWriter(File.OpenWrite("log.txt")))
                {
                    foreach (var log in currentLogs)
                    {
                        if (counter == number) break;
                        file.WriteLine($"[{log.Timestamp}] #{Context.Guild.GetChannel(log.ChannelId).Name} {Context.Guild.GetUser(log.AuthorId).Username}: {log.Content}");
                        foreach (var edit in log.Edits) file.WriteLine($"[{edit.Value}] {edit.Key}");
                        counter++;
                    }
                }


                await Context.Channel.SendFileAsync("log.txt");
                File.Delete("log.txt");

            }
        }
    }
}
