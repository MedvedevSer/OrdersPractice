namespace Orders.Models;

public class OrderItem
{
    public string Name { get; }
    public decimal Price { get; }
    public int Quantity { get; }

    public OrderItem(string name, decimal price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }
}