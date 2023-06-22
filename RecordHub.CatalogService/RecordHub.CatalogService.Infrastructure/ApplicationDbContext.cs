using Microsoft.EntityFrameworkCore;
using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Record> Records { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Style> Styles { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Record>().HasIndex(r => r.Slug);
            modelBuilder.Entity<Label>().HasIndex(l => l.Slug);
            modelBuilder.Entity<Artist>().HasIndex(a => a.Slug);
            modelBuilder.Entity<Country>().HasAlternateKey(c => c.Slug);
            modelBuilder.Entity<Style>().HasAlternateKey(d => d.Slug);
        }
    }
}
