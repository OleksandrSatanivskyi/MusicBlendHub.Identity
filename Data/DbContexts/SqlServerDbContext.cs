using Microsoft.EntityFrameworkCore;
using MusicBlendHub.Identity.Data.EntityTypeConfiguration;
using MusicBlendHub.Identity.Models;

namespace MusicBlendHub.Identity.Data.DbContexts
{
    public class SqlServerDbContext : DbContext, IAuthDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UnconfirmedUserRecord> UnconfirmedUserRecords { get; set; }

        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : base(options) { }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UnconfirmedUserRecordConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
