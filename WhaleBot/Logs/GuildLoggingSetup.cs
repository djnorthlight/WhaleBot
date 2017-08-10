using System;
using System.Collections.Generic;
using System.Text;

namespace WhaleBot
{
    public class GuildLoggingSetup
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong EditChannelId { get; set; }
        public ulong RemoveChannelId { get; set; }
    }
}
