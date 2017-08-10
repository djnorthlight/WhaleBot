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
    public class RequireUserPermissionAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if(context.User.Id == 194538654159339520) return Task.FromResult(PreconditionResult.FromSuccess()); 


            Permission perm;
            var user = context.User as IGuildUser;
            if (user == null) return Task.FromResult(PreconditionResult.FromError("Command must be used in a guild channel"));
            if((user as SocketGuildUser).Roles.Any(x => x.Permissions.Administrator == true)) return Task.FromResult(PreconditionResult.FromSuccess());
            if(user.Id == context.Guild.OwnerId) return Task.FromResult(PreconditionResult.FromSuccess());


            using (var db = new DatabaseContext())
            {
                perm = db.Permissions.FirstOrDefault(x => x.TargetId == context.User.Id && x.GuildId == context.Guild.Id && x.CommandName == command.Name);
                if (perm == null) perm = db.Permissions.FirstOrDefault(x => user.RoleIds.Any() && x.GuildId == context.Guild.Id && x.CommandName == command.Name);
            }
            

            if(perm == null) return Task.FromResult(PreconditionResult.FromError("User doesn't have the required permission"));


            if (perm.IsRole)
            {
                if (user.RoleIds.Any(x => x == perm.TargetId))
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
                else
                {
                    return Task.FromResult(PreconditionResult.FromError("User doesn't have the required permission"));
                }
            }
            else
            {
                if (perm.TargetId == user.Id)
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
                else
                {
                    return Task.FromResult(PreconditionResult.FromError("User doesn't have the required permission"));
                }
            }

        }
    }
}
