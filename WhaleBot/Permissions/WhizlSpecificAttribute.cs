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
    class WhizlSpecificAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Guild.Id == 324282875035779072) return Task.FromResult(PreconditionResult.FromSuccess());
            else return Task.FromResult(PreconditionResult.FromError("This command can only be run on the WhizL server"));
        }

    }
}
