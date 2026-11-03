using System;
using System.Threading.Tasks;
using Moq;
using Orders.Abstractions;
using Orders.Handlers;
using Orders.Models;
using Xunit;

namespace Orders.Tests;

public class OrderCreatedHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidMessage_SendsNotification()
    {
        var sender = new Mock<INotificationSender>();
        var handler = new OrderCreatedHandler(sender.Object);
        var id = Guid.NewGuid();

        await handler.HandleAsync(new OrderCreated(id));

        sender.Verify(
            s => s.SendAsync(It.Is<string>(msg => msg.Contains(id.ToString()))),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_EmptyOrderId_Throws()
    {
        var sender = new Mock<INotificationSender>();
        var handler = new OrderCreatedHandler(sender.Object);

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(new OrderCreated(Guid.Empty)));

        Assert.Contains("OrderId", ex.Message);
        sender.Verify(s => s.SendAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_NullMessage_Throws()
    {
        var handler = new OrderCreatedHandler(Mock.Of<INotificationSender>());
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => handler.HandleAsync(null!));
    }
}