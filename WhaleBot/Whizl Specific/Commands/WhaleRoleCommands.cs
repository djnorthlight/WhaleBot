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

namespace WhaleBot
{
    public class WhaleRoleCommands : ModuleBase
    {
        WhaleRoleMessageHandler WhaleRole;
        public WhaleRoleCommands(IServiceProvider provider)
        {
            this.WhaleRole = (WhaleRoleMessageHandler)provider.GetService(typeof(WhaleRoleMessageHandler));
        }

        [Command("whale")][RequireOwner][WhizlSpecific]
        public async Task WhaleCommand()
        {
            await Task.Run(() => WhaleRole.SendMessage());
        }

        [Command("whale time")][RequireUserPermission][WhizlSpecific]
        public async Task WhaleTimeCommand(IGuildUser user = null)
        {
            if (user == null) user = Context.User as IGuildUser;
            RoleAssignment currentRoleAssignment = null;
            using (var db = new DatabaseContext())
            {
                currentRoleAssignment = db.RoleAssignments.FirstOrDefault(x => x.UserId == user.Id);
            }

            if (currentRoleAssignment == null)
            {
                await ReplyAsync("User isn't a whale");
                return;
            }

            await ReplyAsync($"{user.Mention} has {currentRoleAssignment.HourGiven.AddHours(24) - DateTime.Now} left");
            

            
        }
    }
}
