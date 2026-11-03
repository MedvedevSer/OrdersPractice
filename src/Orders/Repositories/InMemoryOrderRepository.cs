using System.Collections.Concurrent;
using Orders.Abstractions;
using Orders.Models;

namespace Orders.Repositories;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public Task SaveAsync(Order order)
    {
        if (order is null) throw new ArgumentNullException(nameof(order));
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task<Order?> FindAsync(Guid id)
    {
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }
}