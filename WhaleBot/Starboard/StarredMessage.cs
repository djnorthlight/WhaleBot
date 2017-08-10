using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WhaleBot
{
    public class StarredMessage
    {
        public int Id { get; set; }
        public int Stars { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong AuthorId { get; set; }
        public ulong MessageId { get; set; }
        public ulong StarboardMessageId { get; set; }
        public bool IsPinned { get; set; }
        [NotMapped]
        public List<ulong> StarredByIds { get; set; }
        [NotMapped]
        public DateTime Timestamp { get; set; }

        [Column]
        public string TimestampTranslation
        {
            get => JsonConvert.SerializeObject(Timestamp);
            set => Timestamp = JsonConvert.DeserializeObject<DateTime>(value.ToString());
        }


        [Column]
        public string StarredByTranslation
        {
            get => JsonConvert.SerializeObject(StarredByIds);
            set => StarredByIds = JsonConvert.DeserializeObject<List<ulong>>(value.ToString());
        }


        public StarredMessage()
        {
            StarredByIds = new List<ulong>();
        }
    }
}
