using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WhaleBot
{
    public class LoggedMessage
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong AuthorId { get; set; }
        public ulong MessageId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEdited { get; set; }
        [NotMapped]
        public DateTime Timestamp { get; set; }
        public string Content { get; set; }
        [NotMapped]
        public Dictionary<string, DateTime> Edits { get; set; }

        public LoggedMessage()
        {
            this.Edits = new Dictionary<string, DateTime>();
        }

        [Column]
        public string TimestampTranslation
        {
            get => JsonConvert.SerializeObject(Timestamp);
            set => Timestamp = JsonConvert.DeserializeObject<DateTime>(value.ToString());
        }

        [Column]
        public string EditsTranslation
        {
            get => JsonConvert.SerializeObject(Edits);
            set => Edits = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(value.ToString());
        }

    }
}
