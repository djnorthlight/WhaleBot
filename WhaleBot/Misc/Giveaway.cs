using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WhaleBot
{
    public class Giveaway
    {
        public int Id { get; set; }
        [NotMapped]
        public List<ulong> UserIds { get; set; }
        public ulong MessageId { get; set; }
        public ulong GuildId { get; set; }

        [Column]
        public string UserIdsTranslation
        {
            get => JsonConvert.SerializeObject(UserIds);
            set => UserIds = JsonConvert.DeserializeObject<List<ulong>>(value.ToString());
        }

        public Giveaway()
        {
            this.UserIds = new List<ulong>();
        }
    }
}
