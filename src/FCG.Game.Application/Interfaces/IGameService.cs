﻿using FCG.Game.Application.DTOs;

namespace FCG.Game.Application.Interfaces
{
    public interface IGameService
    {
        Task<GameDTO> InsertAsync(CreateGameDTO dto);
        Task<GameDTO> PartialUpdateAsync(string gameId, PartialUpdateGameDTO dto);
        Task<IEnumerable<GameDTO>> GetGamesPaginatedAsync(int page, int size);
        Task<IEnumerable<GameDTO>> GetMostPopularGamesPaginatedAsync(int page, int size);
        Task<IEnumerable<GameDTO>> GetRecommendedGamesPaginatedAsync(int page, int size, Guid userId, string jwt);
        Task<GameDTO> GetByIdAsync(string id);
        Task<bool> DeleteAllAsync();
        Task<bool> DeleteByIdAsync(string id);
        Task<IEnumerable<GameDTO>> IncreasePopularityAsync(IEnumerable<string> gameIds);
    }
}
