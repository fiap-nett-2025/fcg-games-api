using Application.DTOs;

namespace Application.Interfaces;

public interface IGameService
{
    Task<IEnumerable<GameDTO>> GetAllGamesAsync();
    Task<IEnumerable<GameDTO>> GetPromotionalGamesAsync();
    Task<GameDTO> GetGameByIdAsync(int id);
    Task<GameDTO> CreateGameAsync(CreateGameDTO model);
    Task UpdateGameAsync(int id, UpdateGameDTO model);
    Task DeleteGameAsync(int id);
}
