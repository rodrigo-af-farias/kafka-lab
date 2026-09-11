using System.Text.Json;
using KafkaLab.Application.Interfaces;

namespace KafkaLab.Application.Orders;

public sealed class CreateOrderHandler
{
    private readonly IEventPublisher _eventPublisher;

    public CreateOrderHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var integrationEvent = new OrderCreatedIntegrationEvent(
            EventId: Guid.NewGuid(),
            OrderId: command.OrderId,
            OccurredAt: DateTimeOffset.UtcNow);

        var value = JsonSerializer.Serialize(integrationEvent);

        await _eventPublisher.PublishAsync(
            topic: "orders",
            key: command.OrderId.ToString(),
            value: value,
            cancellationToken);
    }
}