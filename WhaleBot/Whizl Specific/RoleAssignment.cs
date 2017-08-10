using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WhaleBot
{
    public class RoleAssignment
    {
        public int Id { get; set; }

        public ulong UserId { get; set; }

        [NotMapped]
        public DateTime HourGiven { get; set; }

        [Column]
        public string HourGivenTranslation
        {
            get => JsonConvert.SerializeObject(HourGiven);
            set => HourGiven = JsonConvert.DeserializeObject<DateTime>(value.ToString());
        }

    }
}
