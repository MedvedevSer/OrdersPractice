using Orders.Abstractions;
using Orders.Models;

namespace Orders.Handlers;

public class OrderCreatedHandler
{
    private readonly INotificationSender _sender;

    public OrderCreatedHandler(INotificationSender sender) => _sender = sender;

    public async Task HandleAsync(OrderCreated message)
    {
        if (message is null) throw new ArgumentNullException(nameof(message));
        if (message.OrderId == Guid.Empty)
            throw new ArgumentException("OrderId cannot be empty", nameof(message));

        await _sender.SendAsync($"Order created: {message.OrderId}");
    }
}