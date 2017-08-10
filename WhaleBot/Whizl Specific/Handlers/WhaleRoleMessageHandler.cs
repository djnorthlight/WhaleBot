//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Discord;
//using Discord.WebSocket;
//using Discord.Commands;
//using System.IO;
//using System.Diagnostics;
//using System.Threading;

//namespace WhaleBot
//{
//    public class WhaleRoleMessageHandler
//    {
//        private readonly Timer _timer;
//        private readonly DiscordSocketClient _client;
//        public static ulong LastMessageId;


//        public WhaleRoleMessageHandler(DiscordSocketClient client)
//        {
//            _client = client;

//            int.TryParse(DateTime.Now.Hour.ToString(), out int HourNow);
//            bool IsSet = int.TryParse(File.ReadAllText(@"\\OLIWIER-PC\ssl log\timer.txt"), out int LastHour);
//            if (!IsSet) LastHour = 0;

//            _timer = new Timer(async _ =>
//            {
//                File.WriteAllText(@"\\OLIWIER-PC\ssl log\timer.txt", DateTime.Now.Hour.ToString());
//                await SendMessage();
//            },
//            null,
//            TimeSpan.FromHours(new Random().Next((HourNow - LastHour) + 1)),  
//            TimeSpan.FromMinutes(new Random().Next(1440, 2880))); 
//        }


//        public async Task SendMessage()
//        {
//            var msg = await _client.GetGuild(324282875035779072).GetTextChannel(324284774527139840).SendMessageAsync("A wild whale has appeared! React to this message to claim your prize for a day");
//            await msg.AddReactionAsync(new Emoji("🐳"));
//            LastMessageId = msg.Id;
//        }


//    }
//}
