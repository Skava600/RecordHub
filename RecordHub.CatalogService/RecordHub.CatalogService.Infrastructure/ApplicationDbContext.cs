using Microsoft.EntityFrameworkCore;
using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Record> Records { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Style> Styles { get; set; }
        public DbSet<RecordStyle> RecordStyles { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecordStyle>()
                .HasKey(r => new { r.StyleId, r.RecordId });
        }
    }
}
