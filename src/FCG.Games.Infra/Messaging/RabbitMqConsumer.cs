using FCG.Games.Domain.Interfaces.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FCG.Games.Infra.Messaging
{
    public class RabbitMqConsumer(ConnectionFactory factory) : IQueueConsumer
    {
        public async Task StartAsync<T>(string queueName, IMessageHandler<T> handler,
            CancellationToken cancellationToken = default)
        {
            using IConnection connection = await factory.CreateConnectionAsync(cancellationToken);
            using IChannel channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<T>(json)
                        ?? throw new InvalidOperationException("Failed to deserialize message");

                    await handler.HandleAsync(message, cancellationToken);
                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception)
                {
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
    }
}