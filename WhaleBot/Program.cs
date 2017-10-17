using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace WhaleBot
{
    public class Program
    {

        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider provider;
        internal GoogleEmbeds embeds;
        bool IsDev = false;



        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 100,
            });

            commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info
            });

            //client.Log += Log;
            client.Ready += Client_Ready;

            embeds = new GoogleEmbeds();
            provider = ConfigureServices();

            commands.AddTypeReader<TimeSpan?>(new TimeSpanReader()); 


            string token = IsDev ? File.ReadAllText(@"Tokens\whalebotdev.txt") : File.ReadAllText(@"Tokens\whalebot.txt");

            await InstallCommands();


            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            Console.Title = "WhaleBot";
            await Task.Delay(-1);
        }

        private async Task Client_Ready()
        {
            await client.SetGameAsync("?help");
        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }



        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(new ReactionHandler(client))
                .AddSingleton(new Logger(client, commands))
                .AddSingleton(embeds)
                .AddSingleton(new WhaleRoleMessageHandler(client))
                .AddSingleton(new WhaleRoleRemovalHandler(client))
                .AddSingleton(new WhaleRoleReactionHandler(client))
                .AddSingleton(new HelpCommands(commands))
                .AddSingleton(new GoogleReactionHandler(client, embeds))
                .AddSingleton(new StaffChannelHandler(client))
                .AddSingleton(new MessageLoggingHandler(client))
                .AddSingleton(new GiveawayHandlers(client))
                .AddSingleton(new StatusUpdatesHandler(client))
                .AddSingleton(new StarboardReactionAddingHandler(client))
                .AddSingleton(new StarboardReactionRemovingHandler(client))
                .AddSingleton(new TosMessageHandler(client))
                .AddSingleton(new JoinAndLeaveLoggingHandler(client))
                .AddSingleton(new ExpiredInfractionsHandler(client))
                .AddSingleton(new RejoinHandler(client))
                .AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false, ThrowOnError = false }));
            var provider = services.BuildServiceProvider();

            return provider;
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {

            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            int argPos = 0;
            if (message.HasMentionPrefix(client.CurrentUser, ref argPos)) await message.Channel.SendMessageAsync("Ur mym");
            if (!(message.HasCharPrefix(IsDev ? '>' : '?', ref argPos))) return;
            var context = new SocketCommandContext(client, message);
            if (message.Channel.GetType() == typeof(SocketDMChannel)) Console.WriteLine(message);
            var result = await commands.ExecuteAsync(context, argPos, provider);


            if (!result.IsSuccess)
            {

                if (result.Error == CommandError.UnknownCommand) return;
                await Logger.Log(new LogMessage(LogSeverity.Info, "Command", $"{context.User.Username} used {messageParam.Content} in {context.Guild.Name} which failed with {result.ErrorReason}"));
                if (result.ErrorReason.Contains("billing")) return;
                await context.Channel.SendMessageAsync($"```ini\n[ERROR] {result.ErrorReason}```");
            }
            else
            {
                await Logger.Log(new LogMessage(LogSeverity.Info, "Command", $"{context.User.Username} used {messageParam.Content} in {context.Guild.Name}"));
            }

        }


        private Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

    }
}