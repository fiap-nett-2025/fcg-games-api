namespace FCG.Games.Domain.Interfaces.Messaging;

public interface IQueueConsumer<T>
{
    Task StartAsync(string queueName, IMessageHandler<T> handler, CancellationToken cancellationToken = default);
}
