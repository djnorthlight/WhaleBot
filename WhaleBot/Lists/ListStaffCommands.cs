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
using System.Runtime.InteropServices;

namespace WhaleBot
{
    public class ListStaffCommands : ModuleBase<SocketCommandContext>
    {
        [Command("list staff", RunMode = RunMode.Async)][Alias("ls")][Summary("Lists all the members with a staff role")]
        [RequireUserPermission]
        public async Task ListStaffCommand([Optional]string mobile)
        {
            await Context.Guild.DownloadUsersAsync();

            var staff = Context.Guild.Users.Where(x => x.Roles.Contains(Context.Guild.Roles.Where(y => y.Name.ToLower() == "staff").First()));
            List<string> roles = new List<string>();
            Dictionary<IRole, List<SocketGuildUser>> rolusers = new Dictionary<IRole, List<SocketGuildUser>>();

            foreach (SocketGuildUser user in staff)
            {
                var highestrole = user.Roles.OrderBy(x => x.Position); 

                if (!(roles.Contains(highestrole.Max().Name)))
                {
                    roles.Add(highestrole.Max().Name);
                }


            }

                foreach (string s in roles)
            {
                var role = Context.Guild.Roles.Where(x => x.Name == s);
                List<SocketGuildUser> dummy = new List<SocketGuildUser>();
                rolusers.Add(role.First(), dummy);
            }

            foreach (SocketGuildUser user in staff)
            {

                foreach (IRole role in user.Roles)
                {


                    if (!roles.Contains(role.Name)) roles.Add(role.Name);
                    var gud = new Dictionary<IRole, List<SocketGuildUser>>(rolusers);
                    try { rolusers.Clear(); } catch { }


                    foreach(KeyValuePair<IRole, List<SocketGuildUser>> c in gud)
                    {
                        if(c.Key == role)
                        {
                            c.Value.Add(user);
                        }
                        rolusers.Add(c.Key, c.Value);
                    }

                }
            }

            var gud2 = new Dictionary<IRole, List<SocketGuildUser>>(rolusers);
            var ordered = gud2.Keys.OrderByDescending(x => x.Position);
            rolusers.Clear();
            
            foreach(var cu in ordered)
            {
                gud2.TryGetValue(cu, out List<SocketGuildUser> o);               
                rolusers.Add(cu, o);
            }



            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { Name = Context.User.Username, IconUrl = Context.User.GetAvatarUrl() },
                Title = "Staff list",
                Description = "Below are all the staff members sorted by rank",
                Color = new Color(178, 224, 40),
            };

            foreach(KeyValuePair<IRole, List<SocketGuildUser>> c in rolusers)
            {
                string value = "";
                foreach(SocketGuildUser user in c.Value)
                {
                    var nickuser = user.Nickname ?? user.Username;
                    var ismobile = mobile == "mobile" ? nickuser : user.Mention;
                    value += ismobile + " " + StaffListHelper.Status(user.Status) + "\n";
                }
                embed.AddField(new EmbedFieldBuilder
                {
                    IsInline = false,
                    Name = c.Key.Name,
                    Value = value,
                });
            }
            embed.WithUrl("http://heeeeeeeey.com/");
            await ReplyAsync("", false, embed);



            
        }
    }
}
