using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Orders.Abstractions;
using Orders.Models;
using Orders.Services;
using Xunit;

namespace Orders.Tests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _repo = new();
    private readonly Mock<IMessagePublisher> _pub = new();
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        // Передаем моки в сервис
        _sut = new OrderService(_repo.Object, _pub.Object, new DiscountCalculator());
    }

    private static NewOrder Cmd() => new(new List<OrderItem>
    {
        new("Книга", 300m, 2)
    });

    [Fact]
    public async Task CreateAsync_ValidOrder_SavesOrder()
    {
        await _sut.CreateAsync(Cmd());
        _repo.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ValidOrder_PublishesEventWithSameId()
    {
        var id = await _sut.CreateAsync(Cmd());
        _pub.Verify(p => p.PublishAsync(It.Is<OrderCreated>(e => e.OrderId == id)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SaveFails_DoesNotPublish()
    {
        _repo.Setup(r => r.SaveAsync(It.IsAny<Order>()))
             .ThrowsAsync(new TimeoutException());

        await Assert.ThrowsAsync<TimeoutException>(() => _sut.CreateAsync(Cmd()));
        _pub.Verify(p => p.PublishAsync(It.IsAny<OrderCreated>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_NullCommand_Throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.CreateAsync(null!));
    }

    [Fact]
    public async Task CreateAsync_EmptyItems_ThrowsAndDoesNotSave()
    {
        var empty = new NewOrder(new List<OrderItem>());

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(empty));
        _repo.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task GetPayableAsync_OrderNotFound_Throws()
    {
        _repo.Setup(r => r.FindAsync(It.IsAny<Guid>())).ReturnsAsync((Order?)null);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetPayableAsync(Guid.NewGuid()));
    }

    // Бонусные тесты на расчет скидки (заглушки)
    [Fact]
    public async Task GetPayableAsync_TotalBelowThreshold_NoDiscount()
    {
        var order = new Order();
        order.Add("X", 500m, 1);
        _repo.Setup(r => r.FindAsync(order.Id)).ReturnsAsync(order);

        Assert.Equal(500m, await _sut.GetPayableAsync(order.Id));
    }

    [Fact]
    public async Task GetPayableAsync_TotalAboveThreshold_AppliesDiscount()
    {
        var order = new Order();
        order.Add("X", 1500m, 1);
        _repo.Setup(r => r.FindAsync(order.Id)).ReturnsAsync(order);

        Assert.Equal(1350m, await _sut.GetPayableAsync(order.Id));
    }
}