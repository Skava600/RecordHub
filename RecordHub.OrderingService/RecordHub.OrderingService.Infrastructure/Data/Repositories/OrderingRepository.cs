using Microsoft.EntityFrameworkCore;
using RecordHub.OrderingService.Domain.Entities;
using RecordHub.Shared.Enums;

namespace RecordHub.OrderingService.Infrastructure.Data.Repositories
{
    public class OrderingRepository : IOrderingRepository
    {
        private OrderingDbContext _context { get; }
        private readonly DbSet<Order> orders;
        public OrderingRepository(OrderingDbContext context)
        {
            orders = context.Orders;
            _context = context;
        }
        public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            await orders.AddAsync(order, cancellationToken);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await GetAsync(id, cancellationToken);
            if (order != null)
            {
                if (_context.Entry(order).State == EntityState.Detached)
                {
                    orders.Attach(order);
                }
                orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await orders.FindAsync(id);
            return order;
        }

        public async Task<IEnumerable<Order>> GetUsersOrdersAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await orders.Where(o => o.UserId.Equals(userId)).Include(o => o.Items).AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task UpdateStateAsync(Guid orderId, StatesEnum state, CancellationToken cancellationToken = default)
        {
            var order = await GetAsync(orderId, cancellationToken);
            if (order != null)
            {
                order.State = state;
                orders.Update(order);
                await _context.SaveChangesAsync(cancellationToken);
            }

        }
    }
}
