using FCG.Game.Application.DTOs;

namespace FCG.Game.Application.Interfaces
{
    public interface IGameService
    {
        Task<GameDTO> InsertAsync(CreateGameDTO dto);
        Task<GameDTO> PartialUpdateAsync(string gameId, PartialUpdateGameDTO dto);
        Task<GameDTO> GetByIdAsync(string id);
        Task<bool> DeleteAllAsync();
        Task<bool> DeleteByIdAsync(string id);
        Task<GameDTO> IncreasePopularity(string gameId);
    }
}
