using System;
using System.Collections.Generic;
using System.Text;

namespace WhaleBot
{
    public class GuildSetup
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong EditChannelId { get; set; }
        public ulong RemoveChannelId { get; set; }
        public ulong JoinChannelId { get; set; }
        public ulong LeaveChannelId { get; set; }
        public ulong MutedRoleId { get; set; }
        public ulong ModChannelId { get; set; }
    }
}
