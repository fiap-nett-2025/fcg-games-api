using AutoMapper;
using Azure;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using FCG.Game.Application.DTOs;
using FCG.Game.Application.Exceptions;
using FCG.Game.Application.Interfaces;
using FCG.Game.Domain.Entities;

namespace FCG.Game.Application.Services
{
    public class GameService(ElasticsearchClient client, IMapper mapper) : IGameService
    {
        const string GAME_ELASTIC_SEARCH_INDEX = "search-97tr";

        public async Task<GameDTO> InsertAsync(CreateGameDTO dto)
        {
            Domain.Entities.Game.ValidateTitle(dto.Title);
            Domain.Entities.Game.ValidatePrice(dto.Price);
            Domain.Entities.Game.ValidateDescription(dto.Description);
            Domain.Entities.Game.ValidateGenreList(dto.Genre);
            var game = mapper.Map<Domain.Entities.Game>(dto);

            var response = await client.IndexAsync(game, i => i
                    .Index(GAME_ELASTIC_SEARCH_INDEX)
                    .Id(game.Id)
                    .OpType(OpType.Create));

            if (!response.IsValidResponse)
            {
                var errorMessage = response.ElasticsearchServerError?.ToString() ?? response.DebugInformation;
                throw new InvalidOperationException($"Falha ao indexar jogo em '{GAME_ELASTIC_SEARCH_INDEX}': {errorMessage}");
            }
            
            return new GameDTO()
            {
                Id = response.Id,
                Description = game.Description,
                Genre = game.Genre,
                Price = game.Price,
                Title = game.Title,
                Popularity = game.Popularity
            };
        }

        public async Task<IEnumerable<GameDTO>> GetGamesPaginated(int page, int size)
        {
            var response = await client.SearchAsync<Domain.Entities.Game>(s => s
                .Indices(GAME_ELASTIC_SEARCH_INDEX)
                .From((page - 1) * size)
                .Size(size)
            );

            var games = response.Hits.Select(game => new GameDTO
            {
                Id = game.Id,
                Title = game.Source.Title,
                Description = game.Source.Description,
                Genre = game.Source.Genre,
                Price = game.Source.Price,
                Popularity = game.Source.Popularity
            });

            return games;
        }

        public async Task<IEnumerable<GameDTO>> GetMostPopularGamesPaginated(int page, int size)
        {
            var response = await client.SearchAsync<Domain.Entities.Game>(s => s
                .Indices(GAME_ELASTIC_SEARCH_INDEX)
                .From((page - 1) * size)
                .Size(size)
                .Sort(sort => sort
                    .Field(g => g.Popularity, SortOrder.Desc)
                )
            );

            var games = response.Hits.Select(game => new GameDTO
            {
                Id = game.Id,
                Title = game.Source.Title,
                Description = game.Source.Description,
                Genre = game.Source.Genre,
                Price = game.Source.Price,
                Popularity = game.Source.Popularity
            });

            return games;
        }

        public async Task<GameDTO> GetByIdAsync(string id)
        {
            var game = await GetGameByIdAsync(id);
            return new GameDTO()
            {
                Id = id,
                Description = game.Description,
                Genre = game.Genre,
                Price = game.Price,
                Title = game.Title,
                Popularity = game.Popularity
            };
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            var response = await client.DeleteAsync<Domain.Entities.Game>(id, d => d.Index(GAME_ELASTIC_SEARCH_INDEX));

            if (response.ApiCallDetails?.HttpStatusCode == 404)
                throw new NotFoundException("Jogo não encontrado.");

            if (!response.IsValidResponse)
            {
                var detail = response.ElasticsearchServerError?.ToString() ?? response.DebugInformation;
                throw new InvalidOperationException($"Falha ao deletar jogo '{id}' em '{GAME_ELASTIC_SEARCH_INDEX}': {detail}");
            }
            return response.Result == Result.Deleted;
        }

        public async Task<bool> DeleteAllAsync()
        {
            var response = await client.DeleteByQueryAsync(GAME_ELASTIC_SEARCH_INDEX, d => d
                .Query(q => q.MatchAll(new MatchAllQuery()))
                .Conflicts(Conflicts.Proceed)
                .Refresh(true)
                .WaitForCompletion(true)
            );

            if (!response.IsValidResponse)
            {
                var errorMessage = response.ElasticsearchServerError?.ToString() ?? response.DebugInformation;
                throw new InvalidOperationException($"Falha ao apagar documentos do índice '{GAME_ELASTIC_SEARCH_INDEX}': {errorMessage}");
            }

            return response.Deleted.GetValueOrDefault() > 0;
        }

        public async Task<GameDTO> PartialUpdateAsync(string gameId, PartialUpdateGameDTO dto)
        {
            var game = await GetGameByIdAsync(gameId);

            game.Title = dto.Title ?? game.Title;
            game.Price = dto.Price ?? game.Price;
            game.Description = dto.Description ?? game.Description;
            game.Genre = dto.Genre ?? game.Genre;

            Domain.Entities.Game.ValidateTitle(game.Title);
            Domain.Entities.Game.ValidatePrice(game.Price);
            Domain.Entities.Game.ValidateDescription(game.Description);
            Domain.Entities.Game.ValidateGenreList(game.Genre);

            var response = await UpdateData(gameId, game, OpType.Index);
            return new GameDTO()
            {
                Id = response.Id,
                Description = game.Description,
                Genre = game.Genre,
                Price = game.Price,
                Title = game.Title,
                Popularity = game.Popularity
            };
        }

        public async Task<GameDTO> IncreasePopularity(string gameId)
        {
            var game = await GetGameByIdAsync(gameId);
            game.Popularity++;

            var response = await UpdateData(gameId, game, OpType.Index);

            return new GameDTO()
            {
                Id = response.Id,
                Description = game.Description,
                Genre = game.Genre,
                Price = game.Price,
                Title = game.Title,
                Popularity = game.Popularity
            };
        }

        private async Task<IndexResponse> UpdateData( string gameId, Domain.Entities.Game game, OpType operation)
        {
            var response = await client.IndexAsync(game, i => i
                    .Index(GAME_ELASTIC_SEARCH_INDEX)
                    .Id(gameId)
                    .OpType(operation));

            if (!response.IsValidResponse)
            {
                var errorMessage = response.ElasticsearchServerError?.ToString() ?? response.DebugInformation;
                throw new InvalidOperationException($"Falha ao indexar jogo em '{GAME_ELASTIC_SEARCH_INDEX}': {errorMessage}");
            }
            return response;
        }

        private async Task<Domain.Entities.Game> GetGameByIdAsync(string id)
        {
            var response = await client.GetAsync<Domain.Entities.Game>(id, g => g.Index(GAME_ELASTIC_SEARCH_INDEX));
            return (response?.Source)
                ?? throw new NotFoundException("Jogo não encontrado.");
        }
    }
}
