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
    public class StaffListHelper
    {
        public static string Status(UserStatus status)
        {
            switch (status)
            {
                case UserStatus.Online:
                    return "<:onlinestatus:340578894220492810>";

                case UserStatus.Idle:
                    return "<:idlestatus:340578894027423754> ";

                case UserStatus.DoNotDisturb:
                    return "<:dndstatus:340578893649936395>";

                case UserStatus.Offline:
                    return "<:offlinestatus:340578893809451009>";
            }
            return string.Empty;
        }
    }
}
