using FCG.Games.Application.DTOs;
using FCG.Games.Application.Interfaces;
using FCG.Games.Domain.Interfaces.Messaging;

namespace FCG.Games.Application.Handler;

public class GameIncreasePopularityHandler(IGameService gameService) : IMessageHandler<MessageDTO>
{
    public async Task HandleAsync(MessageDTO message, CancellationToken cancellationToken = default)
    {
        await gameService.IncreasePopularityAsync(message.GamesId);
        Console.WriteLine($"Processing games for user {message.UserId}");
    }
}
