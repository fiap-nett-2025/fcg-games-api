using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGameElasticSearchService
    {
        Task<long> DeleteAllAsync(CancellationToken ct = default);
        Task<string> IndexGameAsync(Game game, CancellationToken ct = default);
        Task<Game?> GetByIdAsync(string id, CancellationToken ct = default);
        Task<bool> DeleteByIdAsync(string id, CancellationToken ct = default);

    }
}
