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
using Microsoft.EntityFrameworkCore;

namespace WhaleBot
{
    public class Permission
    {      
        public int Id { get;  set; }
        public ulong TargetId { get;  set; }
        public ulong GuildId { get;  set; }
        public string CommandName { get;  set; }
        public bool IsRole { get; set; }

        public Permission()
        {

        }

        public Permission(ulong TargetId, ulong GuildId, string CommandName, bool IsRole)
        {
            this.TargetId = TargetId;
            this.GuildId = GuildId;
            this.CommandName = CommandName;
            this.IsRole = IsRole;
        }
    }
}
