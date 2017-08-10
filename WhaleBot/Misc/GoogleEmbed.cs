using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.Customsearch.v1.Data;
using Google.Apis.Services;
using Google.Apis.Customsearch.v1;
using Discord;
using System.IO;

namespace WhaleBot
{
    public class GoogleEmbed
    {
        private static CustomsearchService Search;
        private static string SearchEngineId;


        public int LastIndex { get; set; }
        public Dictionary<int, List<Result>> Pages { get; set; }
        public int ResultsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public ulong MessageId { get; set; }
        public string Query { get; set; }
        public List<Result> NextResults
        {
            get
            {
                var listRequest = Search.Cse.List(this.Query);
                listRequest.Cx = SearchEngineId;
                listRequest.Start = this.LastIndex;
                var search = (listRequest.Execute()).Items;
                
                this.LastIndex = this.LastIndex + this.ResultsPerPage;
                return search as List<Result>;
            }
        }

        static GoogleEmbed()
        {
            string apiKey = File.ReadAllText(@"Tokens\google.txt");           
            SearchEngineId = File.ReadAllText(@"Tokens\googlesearch.txt");
            Search = new CustomsearchService(new BaseClientService.Initializer { ApiKey = apiKey });
        }
        
        public Embed GetEmbed(bool next)
        {
            int nextpage;
            if (next) { nextpage = CurrentPage + 1; } else { nextpage = CurrentPage - 1; }
            CurrentPage = nextpage;
            string reply = "";
            int counter = 0;
            foreach (var result in GetPage(nextpage))
            {
                if (counter <= this.ResultsPerPage) reply += $"[{result.Title}]({result.Link}) \n";
                counter++;
            }
            var embed = new EmbedBuilder
            {
                Description = reply,
                Color = new Color(178, 224, 40),
                Footer = new EmbedFooterBuilder { Text = $"Page: {CurrentPage}"}
            }.WithUrl("http://heeeeeeeey.com/").Build();
            return embed;
        }

        private List<Result> GetPage(int id)
        {
            if(!Pages.TryGetValue(id, out List<Result> page))
            {
                var next = this.NextResults;
                Pages.Add(id, next);
                return next;
            }

            return page;
        }

    }
}
