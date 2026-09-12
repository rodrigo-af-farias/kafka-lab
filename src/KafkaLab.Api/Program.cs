using KafkaLab.Application.Interfaces;
using KafkaLab.Application.Orders;
using KafkaLab.Infrastructure.Kafka;

var builder = WebApplication.CreateBuilder(args);

var kafkaBootstrapServers =
    Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS")
    ?? "localhost:9092";

var kafkaSaslUsername =
    Environment.GetEnvironmentVariable("KAFKA_SASL_USERNAME");

var kafkaSaslPassword =
    Environment.GetEnvironmentVariable("KAFKA_SASL_PASSWORD");

builder.Services.AddSingleton<IEventPublisher>(
    new KafkaEventPublisher(
        kafkaBootstrapServers,
        kafkaSaslUsername,
        kafkaSaslPassword));

builder.Services.AddScoped<CreateOrderHandler>();

var app = builder.Build();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        Status = "Healthy"
    });
});

app.MapGet("/instance", () =>
{
    return Results.Ok(new
    {
        Instance = Environment.MachineName
    });
});

app.MapPost("/orders/{orderId:guid}", async (
    Guid orderId,
    CreateOrderHandler handler,
    CancellationToken cancellationToken) =>
{
    var command = new CreateOrderCommand(orderId);

    await handler.HandleAsync(
        command,
        cancellationToken);

    return Results.Ok(new
    {
        OrderId = orderId
    });
});

app.Run();