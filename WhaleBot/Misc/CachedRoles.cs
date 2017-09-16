using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WhaleBot
{
    public class CachedRoles
    {
        public int Id { get; set; }
        public ulong UserId { get; set; }
        [NotMapped]
        public List<ulong> RoleIds { get; set; }

        [Column]
        public string RoleIdsTranslation
        {
            get => JsonConvert.SerializeObject(RoleIds);
            set => RoleIds = JsonConvert.DeserializeObject<List<ulong>>(value.ToString());
        }
    }
}
