using KafkaLab.Application.Interfaces;
using MediatR;

namespace KafkaLab.Application.Orders.ProcessOrderCreated;

public sealed class ProcessOrderCreatedHandler
    : IRequestHandler<ProcessOrderCreatedCommand>
{
    private readonly IProcessedEventRepository _processedEventRepository;

    public ProcessOrderCreatedHandler(
        IProcessedEventRepository processedEventRepository)
    {
        _processedEventRepository = processedEventRepository;
    }

    public async Task Handle(
        ProcessOrderCreatedCommand request,
        CancellationToken cancellationToken)
    {
        var integrationEvent = request.IntegrationEvent;

        var markedAsProcessed =
            await _processedEventRepository.TryMarkAsProcessedAsync(
                integrationEvent.EventId,
                cancellationToken);

        if (!markedAsProcessed)
        {
            Console.WriteLine(
                $"Evento duplicado ignorado: {integrationEvent.EventId}");

            return;
        }

        Console.WriteLine(
            $"Processando OrderCreated: " +
            $"EventId={integrationEvent.EventId} | " +
            $"OrderId={integrationEvent.OrderId}");

        // Futuro processamento da ordem ficará aqui.
    }
}