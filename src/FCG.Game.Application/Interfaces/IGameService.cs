using FCG.Game.Application.DTOs;

namespace FCG.Game.Application.Interfaces
{
    public interface IGameService
    {
        Task<GameDTO> InsertAsync(CreateGameDTO dto);
        Task<GameDTO> PartialUpdateAsync(string gameId, PartialUpdateGameDTO dto);
        Task<IEnumerable<GameDTO>> GetGamesPaginated(int page, int size);
        Task<IEnumerable<GameDTO>> GetMostPopularGamesPaginated(int page, int size);
        Task<IEnumerable<GameDTO>> GetRecommendedGamesPaginated(int page, int size, Guid userId, string jwt);
        Task<GameDTO> GetByIdAsync(string id);
        Task<bool> DeleteAllAsync();
        Task<bool> DeleteByIdAsync(string id);
        Task<IEnumerable<GameDTO>> IncreasePopularity(IEnumerable<string> gameIds);
    }
}
