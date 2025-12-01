using FCG.Games.Application.DTOs;
using FCG.Games.Domain.Interfaces.Messaging;
using FCG.Games.Infra.Config;
using Microsoft.Extensions.Options;

namespace FCG.Games.Worker;

public class GamePopularityConsumerWorker : BackgroundService
{
    private readonly IQueueConsumer<MessageDTO> _consumer;
    private readonly IMessageHandler<MessageDTO> _handler;
    private readonly QueuesOptions _queues;

    public GamePopularityConsumerWorker(
        IQueueConsumer<MessageDTO> consumer,
        IMessageHandler<MessageDTO> handler,
        IOptions<QueuesOptions> queuesOptions)
    {
        _consumer = consumer;
        _handler = handler;
        _queues = queuesOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.StartAsync(
            _queues.GamePopularityIncreasedQueue,
            _handler,
            stoppingToken);
    }
}
