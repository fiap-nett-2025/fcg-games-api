using FCG.Games.Domain.Entities;

namespace FCG.Games.Application.Interfaces;

public interface IGameRecommendationService
{
    Task<IEnumerable<Game>> GetRecommendedGamesPaginatedAsync(int page, int size, Guid userId, string jwt);
}
