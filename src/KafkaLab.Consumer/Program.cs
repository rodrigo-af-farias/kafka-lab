using System.Text.Json;
using Confluent.Kafka;
using KafkaLab.Application.Interfaces;
using KafkaLab.Application.Orders;
using KafkaLab.Application.Orders.ProcessOrderCreated;
using KafkaLab.Infrastructure.SqlServer;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var kafkaBootstrapServers =
    Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS")
    ?? "localhost:9092";

var kafkaSaslUsername =
    Environment.GetEnvironmentVariable("KAFKA_SASL_USERNAME");

var kafkaSaslPassword =
    Environment.GetEnvironmentVariable("KAFKA_SASL_PASSWORD");

var sqlServerConnectionString =
    Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING")
    ?? "Server=localhost,1433;Database=KafkaLab;User Id=sa;Password=KafkaLab@12345;TrustServerCertificate=True;";

builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssemblyContaining<ProcessOrderCreatedHandler>();
});

builder.Services.AddSingleton<IProcessedEventRepository>(
    new SqlProcessedEventRepository(sqlServerConnectionString));

using var host = builder.Build();

var mediator = host.Services.GetRequiredService<IMediator>();

var config = new ConsumerConfig
{
    BootstrapServers = kafkaBootstrapServers,
    GroupId = "order-processing",
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false
};

if (!string.IsNullOrWhiteSpace(kafkaSaslUsername) &&
    !string.IsNullOrWhiteSpace(kafkaSaslPassword))
{
    config.SecurityProtocol = SecurityProtocol.SaslSsl;
    config.SaslMechanism = SaslMechanism.Plain;
    config.SaslUsername = kafkaSaslUsername;
    config.SaslPassword = kafkaSaslPassword;
}

using var consumer = new ConsumerBuilder<string, string>(config).Build();

consumer.Subscribe("orders");

Console.WriteLine(
    $"Consumer iniciado. Kafka={kafkaBootstrapServers}");

while (true)
{
    var result = consumer.Consume();

    var integrationEvent =
        JsonSerializer.Deserialize<OrderCreatedIntegrationEvent>(
            result.Message.Value);

    if (integrationEvent is null)
    {
        Console.WriteLine("Evento inválido.");
        consumer.Commit(result);
        continue;
    }

    await mediator.Send(
        new ProcessOrderCreatedCommand(integrationEvent));

    consumer.Commit(result);
}