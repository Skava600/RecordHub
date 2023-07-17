using Microsoft.EntityFrameworkCore;
using RecordHub.OrderingService.Domain.Entities;

namespace RecordHub.OrderingService.Infrastructure.Data
{
    public class OrderingDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> Items { get; set; }

        public OrderingDbContext(DbContextOptions<OrderingDbContext> options)
            : base(options)
        {
        }
    }
}
