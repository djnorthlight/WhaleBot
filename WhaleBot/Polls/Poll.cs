using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.IO;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WhaleBot
{
    public class Poll
    {
        [NotMapped]
        public List<string> Options { get; set; }

        public bool IsMultiple { get; set; }

        public ulong MessageId { get; set; }

        public ulong ChannelId { get; set; }

        public ulong GuildId { get; set; }

        [NotMapped]
        public List<string> Emotes { get; set; }

        [NotMapped]
        public Dictionary<ulong, string> Votes { get; set; }

        public int Id { get; set; }

        [Column]
        public string OptionsAsOption
        {
            get => JsonConvert.SerializeObject(Options);
            set => Options = JsonConvert.DeserializeObject<List<String>>(value.ToString());
        }


        [Column]
        public string EmotesAsEmote
        {
            get => JsonConvert.SerializeObject(Emotes);
            set => Emotes = JsonConvert.DeserializeObject<List<String>>(value.ToString());
        }


        [Column]
        public string VotesAsVote
        {
            get => JsonConvert.SerializeObject(Votes);
            set => Votes = JsonConvert.DeserializeObject<Dictionary<ulong, string>>(value.ToString());
        }
    }
}
