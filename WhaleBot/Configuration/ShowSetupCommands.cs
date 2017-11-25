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
    public class ShowSetupCommands : ModuleBase<SocketCommandContext>
    {
        [Command("setup")]
        [Summary("Sets the configuration")]
        [Remarks("Exclude from help")]
        [RequireUserPermission]
        public async Task SetupCommand()
        {
            GuildSetup setup;
            using(var db = new DatabaseContext())
            {
                setup = db.GuildSetups.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
            }

            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { IconUrl = Context.User.GetAvatarUrl(), Name = Context.User.Username },
                Title = "Guild configuration",
                Color = new Color(178, 224, 40),
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder { Name = "edit_log", Value = setup.EditChannelId == 0 ? "Not set" : Context.Guild.GetTextChannel(setup.EditChannelId)?.Mention, IsInline = true  },
                    new EmbedFieldBuilder { Name = "delete_log", Value = setup.RemoveChannelId == 0 ? "Not set" : Context.Guild.GetTextChannel(setup.RemoveChannelId)?.Mention, IsInline = true  },
                    new EmbedFieldBuilder { Name = "join_log", Value = setup.JoinChannelId == 0 ? "Not set" : Context.Guild.GetTextChannel(setup.JoinChannelId)?.Mention, IsInline = true  },
                    new EmbedFieldBuilder { Name = "leave_log", Value = setup.LeaveChannelId == 0 ? "Not set" : Context.Guild.GetTextChannel(setup.LeaveChannelId)?.Mention, IsInline = true  },
                    new EmbedFieldBuilder { Name = "mod_log", Value = setup.ModChannelId == 0 ? "Not set" : Context.Guild.GetTextChannel(setup.ModChannelId)?.Mention, IsInline = true },
                    new EmbedFieldBuilder { Name = "muted_role", Value = setup.MutedRoleId == 0 ? "Not set" : Context.Guild.GetRole(setup.MutedRoleId)?.Mention, IsInline = true },
                }
            };

            await ReplyAsync("", false, embed);
        }
    }
}
