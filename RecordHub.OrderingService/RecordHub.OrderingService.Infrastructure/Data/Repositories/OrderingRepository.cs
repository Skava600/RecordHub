﻿using Microsoft.EntityFrameworkCore;
using RecordHub.OrderingService.Domain.Entities;

namespace RecordHub.OrderingService.Infrastructure.Data.Repositories
{
    public class OrderingRepository : IOrderingRepository
    {
        private readonly OrderingDbContext _context;
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
            return await orders
                .Where(o => o.UserId.Equals(userId))
                .Include(o => o.Items)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            orders.Update(order);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
