using Orders.Models;
using Xunit;

namespace Orders.Tests;

public class OrderTests
{
    // --- ГОТОВЫЙ ТЕСТ (оставляем как было) ---
    [Fact]
    public void Total_TwoItems_ReturnsSum()
    {
        var order = new Order();
        order.Add("Книга", 300m, 2);
        order.Add("Ручка", 50m, 1);

        var total = order.Total();

        Assert.Equal(650m, total);
    }

    // --- НОВЫЕ ТЕСТЫ ---

    [Fact]
    public void Total_EmptyOrder_ReturnsZero()
    {
        var order = new Order();
        Assert.Equal(0m, order.Total());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Add_InvalidName_Throws(string? name)
    {
        var order = new Order();
        Assert.Throws<ArgumentException>(() => order.Add(name!, 100m, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Add_NonPositivePrice_Throws(decimal price)
    {
        var order = new Order();
        Assert.Throws<ArgumentOutOfRangeException>(() => order.Add("Item", price, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void Add_InvalidQuantity_Throws(int qty)
    {
        var order = new Order();
        Assert.Throws<ArgumentOutOfRangeException>(() => order.Add("Item", 100m, qty));
    }

    [Fact]
    public void Add_ValidItem_AddsToItems()
    {
        var order = new Order();
        order.Add("Книга", 300m, 2);

        var item = Assert.Single(order.Items);
        Assert.Equal("Книга", item.Name);
        Assert.Equal(300m, item.Price);
        Assert.Equal(2, item.Quantity);
    }
}