using KafkaLab.Application.Orders;
using MediatR;

namespace KafkaLab.Application.Orders.ProcessOrderCreated;

public sealed record ProcessOrderCreatedCommand(
    OrderCreatedIntegrationEvent IntegrationEvent) : IRequest;