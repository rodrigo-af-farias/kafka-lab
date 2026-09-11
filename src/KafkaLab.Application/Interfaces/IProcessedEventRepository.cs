namespace KafkaLab.Application.Interfaces;

public interface IProcessedEventRepository
{
    Task<bool> TryMarkAsProcessedAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);
}