using System;
using System.Collections.Generic;
using System.Text;

namespace WhaleBot
{
    public class GuildStarringSetup
    {
        public int Id { get; set; }
        public ulong StarboardChannelId { get; set; }
        public ulong GuildId { get; set; }
        public int StarsRequired { get; set; }
    }
}
