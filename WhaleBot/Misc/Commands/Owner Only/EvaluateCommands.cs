using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace WhaleBot
{
    public class Deva : ModuleBase
    {

        [Command("gc")][RequireOwner]
        [Alias("collectgarbage")]
        [Summary("Forces the GC to clean up resources")]
        public async Task CollectGarbage()
        {
            GC.Collect();
            // TODO: Replace emoji
            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "GC",
                Color = new Color(0, 255, 0),
                Description = ":thumbsup:"
            });
        }

        [Command("eval", RunMode = RunMode.Async), RequireOwner]
        [Alias("cseval", "csharp", "evaluate")]
        [Summary("Evaluates C# code")]
        public async Task Eval([Remainder]string input)
        {
            int index1 = input.IndexOf('\n', input.IndexOf("```") + 3) + 1;
            int index2 = input.LastIndexOf("```");

            if (index1 == -1 || index2 == -1)
                throw new ArgumentException("You need to wrap the code into a code block.");
            string code = input.Substring(index1, index2 - index1);

            Task<IUserMessage> msg = ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "Evaluation",
                Color = new Color(0, 255, 0),
                Description = "Evaluating..."
            });

            try
            {
                ScriptOptions options = ScriptOptions.Default
                    .WithReferences(Assembly.GetEntryAssembly().Location)
                    .AddImports(new[]
                    {
                        "Discord",
                        "Discord.Commands",
                        "Discord.WebSocket",
                        "System",
                        "System.Linq",
                        "System.Collections",
                        "System.Collections.Generic",
                        "System.Threading.Tasks",
                        "System.IO",
                        "System.Text"
                    });

                object result = await CSharpScript.EvaluateAsync(code, options, globals:
                    new RoslynGlobals()
                    {
                        Client = Context.Client as DiscordSocketClient,
                        Channel = Context.Channel as SocketTextChannel,
                        Guild = Context.Guild as SocketGuild
                    }
                );

                await (await msg).ModifyAsync(x => x.Embed = new EmbedBuilder
                {
                    Title = "Evaluation",
                    Description = result?.ToString() ?? "Success, nothing got returned",
                    Color = new Color(0, 255, 0)
                }.Build());
            }
            catch (Exception ex)
            {
                await (await msg).ModifyAsync(x => x.Embed = new EmbedBuilder
                {
                    Title = "Evaluation Failure",
                    Description = $"**{ex.GetType()}**: {ex.Message}",
                    Color = new Color(255, 0, 0)
                }.Build());
            }
        }

        public class RoslynGlobals
        {
            public DiscordSocketClient Client { get; set; }
            public SocketTextChannel Channel { get; set; }
            public SocketGuild Guild { get; set; }
        }
    }
}
