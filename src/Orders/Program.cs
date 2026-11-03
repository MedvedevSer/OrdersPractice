using System;
using System.Threading.Tasks;
using Orders.Abstractions;
using Orders.Models;
using Orders.Repositories;
using Orders.Services;
using Orders.Handlers;

namespace Orders;

public class Program
{
    // Точка входа в программу (Main)
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Демонстрация работы приложения ===");

        // 1. Создаем зависимости
        IOrderRepository repository = new InMemoryOrderRepository();
        IMessagePublisher publisher = new ConsolePublisher();
        var discountCalculator = new DiscountCalculator();

        // 2. Создаем сервис
        var orderService = new OrderService(repository, publisher, discountCalculator);

        // 3. Формируем заказ
        var newOrder = new NewOrder(new List<OrderItem>
        {
            new("Книга", 300m, 2),
            new("Ручка", 50m, 1)
        });

        // 4. Создаем заказ (сохраняем и публикуем событие)
        Console.WriteLine("Создание заказа...");
        var orderId = await orderService.CreateAsync(newOrder);
        Console.WriteLine($"Заказ создан с Id: {orderId}");

        // 5. Проверяем расчет к оплате
        var payable = await orderService.GetPayableAsync(orderId);
        Console.WriteLine($"Сумма к оплате (с учетом скидки): {payable} руб.");

        // 6. Демонстрация работы обработчика
        var notificationSender = new ConsoleNotificationSender();
        var handler = new OrderCreatedHandler(notificationSender);
        await handler.HandleAsync(new OrderCreated(orderId));

        Console.WriteLine("=== Демонстрация завершена ===");
    }
}

// Реализация публикатора сообщений (выводит в консоль)
public class ConsolePublisher : IMessagePublisher
{
    public Task PublishAsync(OrderCreated message)
    {
        Console.WriteLine($"[БРОКЕР] Опубликовано событие: Создан заказ {message.OrderId}");
        return Task.CompletedTask;
    }
}

// Реализация отправителя уведомлений (для обработчика)
public class ConsoleNotificationSender : INotificationSender
{
    public Task SendAsync(string message)
    {
        Console.WriteLine($"[УВЕДОМЛЕНИЕ] {message}");
        return Task.CompletedTask;
    }
}