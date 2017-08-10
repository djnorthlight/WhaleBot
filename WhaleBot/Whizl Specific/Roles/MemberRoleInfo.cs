using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WhaleBot
{
    public class MemberRoleInfo
    {
        public ulong UserId { get; set; }
        public int DaysActive { get; set; }
        [NotMapped]
        public DateTime LastActive { get; set; }


        [Column]
        public string LastActiveTranslation
        {
            get => JsonConvert.SerializeObject(LastActive);
            set => LastActive = JsonConvert.DeserializeObject<DateTime>(value.ToString());
        }
    }
}
