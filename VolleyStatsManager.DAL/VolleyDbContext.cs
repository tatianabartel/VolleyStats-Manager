using Microsoft.EntityFrameworkCore;
using VolleyStatsManager.DAL.Entities;

namespace VolleyStatsManager.DAL
{
    public class VolleyDbContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchSet> MatchSets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PlayerStats> PlayerStats { get; set; }

        public VolleyDbContext(DbContextOptions<VolleyDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .HasOne(t => t.User)
                .WithMany(u => u.Teams)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<Match>()
                .HasMany(m => m.Sets)
                .WithOne(s => s.Match)
                .HasForeignKey(s => s.MatchId);

            modelBuilder.Entity<PlayerStats>()
                .HasOne(ps => ps.Player)
                .WithMany()
                .HasForeignKey(ps => ps.PlayerId);

            modelBuilder.Entity<PlayerStats>()
                .HasOne(ps => ps.Match)
                .WithMany()
                .HasForeignKey(ps => ps.MatchId);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Team)
                .WithMany(t => t.Matches)
                .HasForeignKey(m => m.TeamId);
        }
    }
}
