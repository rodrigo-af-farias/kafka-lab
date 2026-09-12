using Confluent.Kafka;
using KafkaLab.Application.Interfaces;

namespace KafkaLab.Infrastructure.Kafka;

public sealed class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<string, string> _producer;

    public KafkaEventPublisher(
        string bootstrapServers,
        string? saslUsername = null,
        string? saslPassword = null)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };

        if (!string.IsNullOrWhiteSpace(saslUsername) &&
            !string.IsNullOrWhiteSpace(saslPassword))
        {
            config.SecurityProtocol = SecurityProtocol.SaslSsl;
            config.SaslMechanism = SaslMechanism.Plain;
            config.SaslUsername = saslUsername;
            config.SaslPassword = saslPassword;
        }

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