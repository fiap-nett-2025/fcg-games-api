using Elastic.Clients.Elasticsearch;
using FCG.Games.Application.DTOs;
using FCG.Games.Domain.Entities;

namespace FCG.Games.Application.Mappers
{
    public static class GameMappers
    {
        public static GameDTO MapToDto(Game game) => new()
        {
            Id = game.Id,
            Title = game.Title,
            Description = game.Description,
            Genre = game.Genre,
            Price = game.Price,
            Popularity = game.Popularity
        };

        public static IEnumerable<GameDTO> MapToDTOList(IEnumerable<Game> games) =>
            games.Select(game => new GameDTO
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Genre = game.Genre,
                Price = game.Price,
                Popularity = game.Popularity
            });
        public static IEnumerable<GameDTO> ResponseToGameDTOList(SearchResponse<Game> response) =>
            response.Hits.Select(game => new GameDTO
            {
                Id = game.Id,
                Title = game.Source!.Title,
                Description = game.Source.Description,
                Genre = game.Source.Genre,
                Price = game.Source.Price,
                Popularity = game.Source.Popularity
            });
    }
}
