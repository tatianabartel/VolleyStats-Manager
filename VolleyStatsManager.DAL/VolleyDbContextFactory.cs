using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VolleyStatsManager.DAL
{
    public class VolleyDbContextFactory : IDesignTimeDbContextFactory<VolleyDbContext>
    {
        public VolleyDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<VolleyDbContext>();

            optionsBuilder.UseSqlite("Data Source=volleystats.db");

            return new VolleyDbContext(optionsBuilder.Options);
        }
    }
}