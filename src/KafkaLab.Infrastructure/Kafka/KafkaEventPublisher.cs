using Confluent.Kafka;
using KafkaLab.Application.Interfaces;

namespace KafkaLab.Infrastructure.Kafka;

public sealed class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<string, string> _producer;

    public KafkaEventPublisher(string bootstrapServers)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync(
        string topic,
        string key,
        string value,
        CancellationToken cancellationToken = default)
    {
        var message = new Message<string, string>
        {
            Key = key,
            Value = value
        };

        await _producer.ProduceAsync(
            topic,
            message,
            cancellationToken);
    }
}