using Orders.Validation;

namespace Orders.Models;

public class Order
{
    public Guid Id { get; init; } = Guid.NewGuid();  // ← добавлено
    private readonly List<OrderItem> _items = new();
    public IReadOnlyList<OrderItem> Items => _items;

    public Order() { }

    public Order(IEnumerable<OrderItem> items)
    {
        _items.AddRange(items);
    }

    public void Add(string name, decimal price, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price must be positive");

        if (!QuantityValidator.IsValidQty(quantity))
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be 1..100");

        _items.Add(new OrderItem(name, price, quantity));
    }

    public decimal Total() => _items.Sum(i => i.Price * i.Quantity);
}