using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WhaleBot
{
    public class MemberRoleInfo
    {
        public int Id { get; set; }
        public ulong UserId { get; set; }
        public int DaysActive { get; set; }
        [NotMapped]
        public DateTime NextDay { get; set; }


        [Column]
        public string NextDayTranslation
        {
            get => JsonConvert.SerializeObject(NextDay);
            set => NextDay = JsonConvert.DeserializeObject<DateTime>(value.ToString());
        }
    }
}
