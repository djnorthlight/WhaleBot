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
using Unidecode.NET;

namespace WhaleBot
{
    public class AskCommands : ModuleBase
    {
        [Command("ask")]
        [Summary("Asks the bot a question")]
        [RequireUserPermission]
        public async Task AskCommand([Remainder]string question)
        {
            if (question.ToLower().Unidecode().Contains("cygan") || question.ToLower().Contains("194538654159339520"))
            {
                await Context.Channel.SendMessageAsync("Cygan is amazing...");
                return;
            }
            if (question.ToLower().Unidecode().Contains("c") && question.ToLower().Unidecode().Contains("y") && question.ToLower().Unidecode().Contains("g") && question.ToLower().Unidecode().Contains("a") && question.ToLower().Unidecode().Contains("n"))
            {
                if (question.ToLower().LastIndexOf('c') < question.ToLower().LastIndexOf('y') && question.ToLower().LastIndexOf('g') < question.ToLower().LastIndexOf('a') && question.ToLower().LastIndexOf('a') < question.ToLower().LastIndexOf('n'))
                {
                    await Context.Channel.SendMessageAsync("Cygan is amazing...");
                    return;
                }
            }

            if(question.Contains("🇨") || question.Contains("🇾") || question.Contains("🇬") || question.Contains("🇦") || question.Contains("🇳"))
            {
                await Context.Channel.SendMessageAsync("Cygan is amazing...");
                return;
            }

            Dictionary<string, string[]> Questions = new Dictionary<string, string[]>
            {
                { "how much", new string[] { new Random().Next(int.MaxValue).ToString() } },
                { "how many", new string[] { new Random().Next(int.MaxValue).ToString() } },
                { "why", new string[] { "Because", "Because fuck you", "Because [REDACTED]", "Why not?", "Because suck my dick" } },
                { "how", new string[] { "U cant ok lol", "I wont tell u how ya dip", "How about u stfu", "Not now ya dip", "By sucking my dick" } },
                { "what", new string[] { "UR FUCKING MYM!!!", "Ur mym lol", "A number 5 with extra dip", "IDK lol" } },
                { "when", new string[] { "In the future...", "Yesterday", "Tomorrow", "In the past...", } },
                { "where", new string[] { "In ur myms vageena", "In hell", "In Los Santos", "In ur mym", } }
            };
            foreach (var c in Questions)
            {
                if (question.ToLower().Contains(c.Key))
                {
                    await ReplyAsync(c.Value[new Random().Next(c.Value.Length + 1)]);
                    return;
                }
            }

            if (new Random().Next(100) > 50)
            {
                await Context.Channel.SendMessageAsync("Yes");
            }
            else
            {
                await Context.Channel.SendMessageAsync("No");
            }




        }

    }
}
