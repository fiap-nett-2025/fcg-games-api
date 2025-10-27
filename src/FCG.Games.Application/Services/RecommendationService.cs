using FCG.Games.Application.Interfaces;
using FCG.Games.Domain.Entities;
using FCG.Games.Domain.Interfaces;

namespace FCG.Games.Application.Services;

public class RecommendationService(IGameRepository gameRepository, IUserLibraryClient userLibraryClient) : IGameRecommendationService
{
    public async Task<IEnumerable<Game>> GetRecommendedGamesPaginatedAsync(int page, int size, Guid userId, string jwt)
    {
        var gameIds = await userLibraryClient.GetOwnedGameIdsAsync(userId, jwt);

        if (gameIds.Count == 0)
            return await gameRepository.GetMostPopularPaginatedAsync(page, size);

        var userGames = await gameRepository.GetGamesByIdsAsync(gameIds);
        var mostFrequentGenre = AnalyzeMostFrequentGenre(userGames);

        if (string.IsNullOrEmpty(mostFrequentGenre))
            return await gameRepository.GetMostPopularPaginatedAsync(page, size);

        return await gameRepository.GetGamesByGenreAsync(
            mostFrequentGenre,
            page,
            size,
            excludeIds: gameIds
        );        
    }

    private string? AnalyzeMostFrequentGenre(IEnumerable<Game?> games)
    {
        return games
            .SelectMany(g => g.Genre.Select(genre => genre.ToString()))
            .GroupBy(g => g)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key)
            .Select(g => g.Key)
            .FirstOrDefault();
    }
}
