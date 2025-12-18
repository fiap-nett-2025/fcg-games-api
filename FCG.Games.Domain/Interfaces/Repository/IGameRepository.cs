using FCG.Games.Domain.Entities;

namespace FCG.Games.Domain.Interfaces.Repository;

public interface IGameRepository
{
    Task AddAsync(Game game);
    Task<Game?> GetByIdAsync(string id);
    Task<IEnumerable<Game?>> GetGamesByIdsAsync(List<string> ids);
    Task<List<Game>> GetGamesByGenreAsync(string genre, int page, int size, List<string>? excludeIds = null);
    Task<IEnumerable<Game>> GetPaginatedAsync(int page, int size);
    Task<IEnumerable<Game>> GetMostPopularPaginatedAsync(int page, int size);
    Task<bool> TitleExistsAsync(string title);
    Task<bool> TitleExistsExcludingIdAsync(string title, string excludeId);
    Task UpdateAsync(string id, Game game);
    Task DeleteByIdAsync(string id);
    Task DeleteAllAsync();
}
