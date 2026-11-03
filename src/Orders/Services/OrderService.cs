using Orders.Abstractions;
using Orders.Models;

namespace Orders.Services;

public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly DiscountCalculator _discountCalculator;

    public OrderService(IOrderRepository repository, IMessagePublisher publisher, DiscountCalculator discountCalculator)
    {
        _repository = repository;
        _publisher = publisher;
        _discountCalculator = discountCalculator;
    }

    public async Task<Guid> CreateAsync(NewOrder command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));
        if (command.Items is null || command.Items.Count == 0)
            throw new ArgumentException("Order must have items", nameof(command));

        var order = new Order(command.Items);
        await _repository.SaveAsync(order);
        await _publisher.PublishAsync(new OrderCreated(order.Id));
        return order.Id;
    }

    public async Task<decimal> GetPayableAsync(Guid id)
    {
        var order = await _repository.FindAsync(id);
        if (order is null) throw new KeyNotFoundException($"Order {id} not found");

        var total = order.Total();
        var discount = _discountCalculator.Calculate(total);
        return total - discount;
    }
}