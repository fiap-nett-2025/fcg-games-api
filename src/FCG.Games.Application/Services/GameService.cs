using FCG.Games.Application.Exceptions;
using FCG.Games.Application.DTOs;
using FCG.Games.Application.Interfaces;
using FCG.Games.Domain.Entities;
using FCG.Games.Application.Mappers;
using FCG.Games.Domain.Interfaces.Repository;

namespace FCG.Games.Application.Services
{
    public class GameService(IGameRepository repository, IGameRecommendationService recommendationService) : IGameService
    {
        public async Task<GameDTO> CreateGameAsync(CreateGameDTO dto)
        {
            if (await repository.TitleExistsAsync(dto.Title))
                throw new BusinessErrorDetailsException($"Já existe um jogo com o título '{dto.Title}'.");

            var game = Game.Create(dto.Title, dto.Price, dto.Description, dto.Genre);

            await repository.AddAsync(game);

            return GameMappers.MapToDto(game);
        }
        public async Task<GameDTO> GetByIdAsync(string id)
        {
            var game = await repository.GetByIdAsync(id) ?? throw new NotFoundException("Jogo não encontrado.");
            return GameMappers.MapToDto(game);
        }
        public async Task<IEnumerable<GameDTO>> GetGamesPaginatedAsync(int page, int size)
        {
            var games = await repository.GetPaginatedAsync(page, size);

            return GameMappers.MapToDTOList(games);
        }
        public async Task<IEnumerable<GameDTO>> GetMostPopularGamesPaginatedAsync(int page, int size)
        {
            var games = await repository.GetMostPopularPaginatedAsync(page, size);

            return GameMappers.MapToDTOList(games);
        }
        public async Task<GameDTO> PartialUpdateAsync(string gameId, PartialUpdateGameDTO dto)
        {
            var game = await repository.GetByIdAsync(gameId) ?? throw new NotFoundException("Jogo não encontrado.");

            game.UpdateTitle(dto.Title ?? game.Title);
            game.UpdatePrice(dto.Price ?? game.Price);
            game.UpdateDescription(dto.Description ?? game.Description);
            game.UpdateGenre(dto.Genre ?? game.Genre);

            if (await repository.TitleExistsExcludingIdAsync(game.Title, game.Id))
                throw new BusinessErrorDetailsException($"Já existe um jogo com o título '{game.Title}'.");

            await repository.UpdateAsync(gameId, game);
            return GameMappers.MapToDto(game);
        }
        public async Task<bool> DeleteByIdAsync(string id)
        {
            await repository.DeleteByIdAsync(id);
            return true;
        }
        public async Task<bool> DeleteAllAsync()
        {
            await repository.DeleteAllAsync();
            return true;
        }
        public async Task<IEnumerable<GameDTO>> IncreasePopularityAsync(IEnumerable<string> gameIds)
        {
            var updatedGames = new List<GameDTO>();

            foreach (var gameId in gameIds)
            {
                var game = await repository.GetByIdAsync(gameId);
                
                if(game is not null)
                {
                    game.IncrementPopularity();
                    await repository.UpdateAsync(gameId, game);
                    updatedGames.Add(GameMappers.MapToDto(game));
                }

            }
            return updatedGames;
        }
        public async Task<IEnumerable<GameDTO>> GetRecommendedGamesPaginatedAsync(int page, int size, Guid userId, string jwt)
        {
            var games = await recommendationService.GetRecommendedGamesPaginatedAsync(page, size, userId, jwt);

            return GameMappers.MapToDTOList(games);
        }
    }
}
