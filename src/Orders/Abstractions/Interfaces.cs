using Orders.Models;

namespace Orders.Abstractions;

public interface IOrderRepository
{
    Task SaveAsync(Order order);
    Task<Order?> FindAsync(Guid id);
}

public interface IMessagePublisher
{
    Task PublishAsync(OrderCreated message);
}

public interface INotificationSender
{
    Task SendAsync(string message);
}