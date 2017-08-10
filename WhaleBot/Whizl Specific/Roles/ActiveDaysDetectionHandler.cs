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
    public class ActiveDaysDetectionHandler
    {
        DiscordSocketClient client;
        public ActiveDaysDetectionHandler(DiscordSocketClient client)
        {
            this.client = client;
          //  client.MessageReceived += Client_MessageReceived;
        }

        //TODO: stuff

        //private Task Client_MessageReceived(SocketMessage arg)
        //{
        //    using(var db = new DatabaseContext())
        //    {
        //        if(!db.MemberRoleInfos.Any(x => x.UserId == arg.Author.Id)) db.MemberRoleInfos.Add(new MemberRoleInfo { UserId = arg.Author.Id, DaysActive = 0, LastActive = DateTime.Now });
        //        else
        //        {
        //            db.MemberRoleInfos.FirstOrDefault(x => x.UserId == arg.Author.Id).
        //        }
        //    }
        //}
    }
}
