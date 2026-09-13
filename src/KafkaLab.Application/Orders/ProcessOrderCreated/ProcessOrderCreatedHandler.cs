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
                $"Evento duplicado ignorado: {integrationEvent.EventId}{Environment.NewLine}");

            return;
        }

        Console.WriteLine(
            $"Processando OrderCreated:\n" +
            $"EventId={integrationEvent.EventId}\n" +
            $"OrderId={integrationEvent.OrderId}{Environment.NewLine}");

        // Futuro processamento da ordem ficará aqui.
    }
}