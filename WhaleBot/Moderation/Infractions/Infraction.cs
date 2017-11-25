using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WhaleBot
{
    public class Infraction
    {
        public int Id { get; set; }
        [NotMapped]
        public InfractionType Type { get; set; }
        public string Reason { get; set; }
        public ulong GuildId { get; set; }
        public ulong IssuerId { get; set; }
        public ulong OffenderId { get; set; }
        public bool IsExpired { get; set; }
        public bool IsDeleted { get; set; }
        [NotMapped]
        public DateTime Timestamp { get; set; }
        [NotMapped]
        public TimeSpan? Duration { get; set; }


        [Column]
        public string TypeTranslation
        {
            get => JsonConvert.SerializeObject(Type);
            set => Type = JsonConvert.DeserializeObject<InfractionType>(value.ToString());
        }

        [Column]
        public string TimestampTranslation
        {
            get => JsonConvert.SerializeObject(Timestamp);
            set => Timestamp = JsonConvert.DeserializeObject<DateTime>(value.ToString());
        }

        [Column]
        public string DurationTranslation
        {
            get => JsonConvert.SerializeObject(Duration);
            set => Duration = JsonConvert.DeserializeObject<TimeSpan?>(value.ToString());
        }

        public Infraction() { }

        public Infraction(ulong GuildId, ulong IssuerId, ulong OffenderId, string Reason, InfractionType Type, TimeSpan? Duration = null)
        {
            this.GuildId = GuildId;
            this.IssuerId = IssuerId;
            this.OffenderId = OffenderId;
            this.Reason = Reason;
            this.Type = Type;
            this.Duration = Duration;
            this.Timestamp = DateTime.Now;

            if (Duration == null && Type != InfractionType.Mute) this.IsExpired = true;           
        }
    }
}
