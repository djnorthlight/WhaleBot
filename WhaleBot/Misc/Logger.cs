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

namespace WhaleBot
{
    public class Logger
    {
        public Logger(DiscordSocketClient client, CommandService commands)
        {
            client.Log += Log;
        }


        public static Task Log(LogMessage arg)
        {
            var colour = Console.ForegroundColor;
            switch (arg.Severity)
            {
                case (LogSeverity.Critical):
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;

                case (LogSeverity.Error):
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;

                case (LogSeverity.Warning):
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case (LogSeverity.Verbose):
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;

                case (LogSeverity.Info):
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;

                case (LogSeverity.Debug):
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;                                
            }
            Console.WriteLine(arg.ToString());
            Console.ForegroundColor = colour;
            return Task.CompletedTask;
        }
    }
}
