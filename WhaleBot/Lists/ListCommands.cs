using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.IO;

namespace WhaleBot
{
    public class ListCommands : ModuleBase<SocketCommandContext>
    {
        [Command("list", RunMode = RunMode.Async)]
        [Alias("l")]
        [Summary("Lists all the members")]
        [RequireUserPermission]
        public async Task ListCommand()
        {


            IEnumerable<SocketGuildUser> onusers = null, offusers = null, ronusers, ronoffusers;
            IRole currentrole;
            Dictionary<IRole, int> onrolusers = new Dictionary<IRole, int>();
            Dictionary<IRole, int> offrolusers = new Dictionary<IRole, int>();



            await Context.Guild.DownloadUsersAsync();
            foreach(IRole role in Context.Guild.Roles)
            {
                currentrole = role;
                var iguser = (Context.Guild.Users as IEnumerable<SocketGuildUser>);

                onusers = iguser.Where(x => x.Status == UserStatus.Online || x.Status == UserStatus.Idle || x.Status == UserStatus.DoNotDisturb || x.Status == UserStatus.AFK);

                offusers = iguser.Where(x => x.Status == UserStatus.Offline);

                ronoffusers = offusers.Where(x => x.Roles.ToDictionary(y => y.Position).Keys.Max() == role.Position);
                ronusers = onusers.Where(x => x.Roles.ToDictionary(y => y.Position).Keys.Max() == role.Position);
                if (role != Context.Guild.EveryoneRole && role.IsHoisted && !role.IsManaged)
                {
                    onrolusers.Add(currentrole, ronusers.Count());
                    offrolusers.Add(currentrole, ronoffusers.Count());
                }
            }
            EmbedBuilder embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = "Member list",
                Color = new Color(178, 224, 40),
                Description = $"There is {Context.Guild.MemberCount} users in this guild {onusers.Count()} of which are online and {offusers.Count()} are offline"
            };
            foreach (IRole role in Context.Guild.Roles)
            {
                if (role != Context.Guild.EveryoneRole && role.IsHoisted && !role.IsManaged)
                {
                    onrolusers.TryGetValue(role, out int count);
                    offrolusers.TryGetValue(role, out int offcount);
                    embed.AddInlineField(role.Name, $"Online: {count} Offline: {offcount}");
                } 
            }

            embed.WithUrl("http://heeeeeeeey.com/");
            await ReplyAsync("", false, embed);

            
        }
    }
}
