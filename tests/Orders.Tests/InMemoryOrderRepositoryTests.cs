using System;
using System.Threading.Tasks;
using Orders.Models;
using Orders.Repositories;
using Xunit;

namespace Orders.Tests;

public class InMemoryOrderRepositoryTests
{
    [Fact]
    public async Task SaveAsync_ThenFindAsync_ReturnsSameOrder()
    {
        var repo = new InMemoryOrderRepository();
        var order = new Order();
        order.Add("Книга", 300m, 2);

        await repo.SaveAsync(order);
        var found = await repo.FindAsync(order.Id);

        // Assert.Same проверяет, что это тот же самый объект в памяти
        Assert.Same(order, found);
    }

    [Fact]
    public async Task FindAsync_UnknownId_ReturnsNull()
    {
        var repo = new InMemoryOrderRepository();
        Assert.Null(await repo.FindAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task SaveAsync_Null_Throws()
    {
        var repo = new InMemoryOrderRepository();
        await Assert.ThrowsAsync<ArgumentNullException>(() => repo.SaveAsync(null!));
    }
}