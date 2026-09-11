namespace KafkaLab.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync(
        string topic,
        string key,
        string value,
        CancellationToken cancellationToken = default);
}