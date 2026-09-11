namespace KafkaLab.Application.Orders;

public sealed record OrderCreatedIntegrationEvent(
    Guid EventId,
    Guid OrderId,
    DateTimeOffset OccurredAt);