using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhaleBot
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Poll> Polls { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RoleAssignment> RoleAssignments { get; set; }
        public DbSet<LoggedMessage> LoggedMessages { get; set; }
        public DbSet<GuildSetup> GuildSetups { get; set; }
        public DbSet<GuildStarringSetup> GuildStarringSetups { get; set; }
        public DbSet<StarredMessage> StarredMessages { get; set; }
        public DbSet<WhaleHunterCount> WhaleHunterCounts { get; set; }
        public DbSet<CachedRoles> CachedRoles { get; set; }
        public DbSet<Giveaway> Giveaways { get; set; }
        public DbSet<Infraction> Infractions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=\\OLIWIER-PC\ssl log\DataBase.db");
        }
    }
}
