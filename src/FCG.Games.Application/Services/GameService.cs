using AutoMapper;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.MachineLearning;
using Elastic.Clients.Elasticsearch.QueryDsl;
using FCG.Games.Application.Exceptions;
using FCG.Games.Application.DTOs;
using FCG.Games.Application.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using FCG.Games.Domain.Entities;

namespace FCG.Games.Application.Services
{
    public class GameService(ElasticsearchClient client, IMapper mapper, IHttpClientFactory httpClientFactory) : IGameService
    {
        const string GAME_ELASTIC_SEARCH_INDEX = "search-97tr";

        public async Task<GameDTO> InsertAsync(CreateGameDTO dto)
        {
            Game.ValidateTitle(dto.Title);
            Game.ValidatePrice(dto.Price);
            Game.ValidateDescription(dto.Description);
            Game.ValidateGenreList(dto.Genre);
            var game = mapper.Map<Game>(dto);

            var exists = await ValidateIfTitleIsAlreadyTaken(game.Title);
            if (exists)
            {
                throw new InvalidOperationException($"Já existe um jogo com o título '{game.Title}'.");
            }

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

        public async Task<IEnumerable<GameDTO>> GetGamesPaginatedAsync(int page, int size)
        {
            var response = await client.SearchAsync<Game>(s => s
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

        public async Task<IEnumerable<GameDTO>> GetMostPopularGamesPaginatedAsync(int page, int size)
        {
            var response = await client.SearchAsync<Game>(s => s
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
            var response = await client.DeleteAsync<Game>(id, d => d.Index(GAME_ELASTIC_SEARCH_INDEX));

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

            Game.ValidateTitle(game.Title);
            Game.ValidatePrice(game.Price);
            Game.ValidateDescription(game.Description);
            Game.ValidateGenreList(game.Genre);

            var exists = await ValidateIfTitleIsAlreadyTaken(game.Title, gameId);
            if (exists)
            {
                throw new InvalidOperationException($"Já existe um jogo com o título '{game.Title}'.");
            }

            var response = await UpdateDataAsync(gameId, game, OpType.Index);
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

        public async Task<IEnumerable<GameDTO>> IncreasePopularityAsync(IEnumerable<string> gameIds)
        {
            var updatedGames = new List<GameDTO>();

            foreach (var gameId in gameIds)
            {
                var game = await GetGameByIdAsync(gameId);
                game.Popularity++;

                var response = await UpdateDataAsync(gameId, game, OpType.Index);

                updatedGames.Add(new GameDTO
                {
                    Id = response.Id,
                    Description = game.Description,
                    Genre = game.Genre,
                    Price = game.Price,
                    Title = game.Title,
                    Popularity = game.Popularity
                });
            }

            return updatedGames;
        }
        
        private async Task<IndexResponse> UpdateDataAsync( string gameId, Game game, OpType operation)
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

        private async Task<Game> GetGameByIdAsync(string id)
        {
            var response = await client.GetAsync<Game>(id, g => g.Index(GAME_ELASTIC_SEARCH_INDEX));
            return (response?.Source)
                ?? throw new NotFoundException("Jogo não encontrado.");
        }

        public async Task<bool> ValidateIfTitleIsAlreadyTaken(string title, string? id = null)
        {
            var response = await client.SearchAsync<Game>(s => s
                .Indices(GAME_ELASTIC_SEARCH_INDEX)
                .Size(0)
                .Query(q => q.Bool(b => b
                    .Must(m => m.Term(t => t
                        .Field(x => x.Title)
                        .Value(title)
                    ))
                    .MustNot(mn => mn.Ids(i => i
                        .Values(id)
                    ))
                )
            ));
            
            return response.Total > 0;
        }

        public async Task<IEnumerable<GameDTO>> GetRecommendedGamesPaginatedAsync(int page, int size, Guid userId, string jwt)
        {
            string[] gameIds = await GetGameIdsFromUserLibraryAsync(userId, jwt);

            //se usuario nao tem 
            if (gameIds.Length == 0)
            {
                return await GetMostPopularGamesPaginatedAsync(page, size);
            }

            //pegando os generos  dos jogos q o usuario tem em sua biblioteca
            string? mostFrequentGenre = await GetMostFrequentGameAsync(gameIds);

            SearchResponse<Game> resp = await GetGamesByGenreExcludingGameThatUserHas(gameIds, mostFrequentGenre, page, size);

            if (!resp.IsValidResponse)
                throw new BusinessErrorDetailsException("response inválido");

            return resp.Hits.Select(game => new GameDTO
            {
                Id = game.Id,
                Title = game.Source.Title,
                Description = game.Source.Description,
                Genre = game.Source.Genre,
                Price = game.Source.Price,
                Popularity = game.Source.Popularity
            }).ToArray();
        }

        private async Task<SearchResponse<Game>> GetGamesByGenreExcludingGameThatUserHas(string[] gameIds, string? mostFrequentGenre, int page, int size)
        {
            // Pesquisa no Elastic os jogos do genero mais frequente, desconsiderando os jogos que o usuario ja tem
            //e ordenando por popularidade
            return await client.SearchAsync<Game>(s => s
                .Indices(GAME_ELASTIC_SEARCH_INDEX)
                .From((page - 1) * size)
                .Size(size)
                .Query(q => q.Bool(b => b
                    .Must(m => m.Term(t => t
                        .Field("genre.keyword")
                        .Value(mostFrequentGenre)
                        .CaseInsensitive(true)
                    ))
                    .MustNot(mn => mn.Ids(i => i
                        .Values(gameIds)
                    ))
                )).Sort(sort => sort
                      .Field(g => g.Popularity, SortOrder.Desc)
                )
            );
        }

        private async Task<string?> GetMostFrequentGameAsync(string[] gameIds)
        {
            var allGenres = new List<string>();

            foreach (var gameId in gameIds)
            {
                var response = await client.GetAsync<Game>(gameId, g =>
                    g.Index(GAME_ELASTIC_SEARCH_INDEX));

                if (response.Found && response.Source?.Genre is not null)
                {
                    allGenres.AddRange(response.Source.Genre.Select(g => g.ToString()));
                }
            }

            //pega o genero mais frequente
            var mostFrequentGenre = allGenres
                .GroupBy(g => g)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)          // desempate 
                .Select(g => g.Key)
                .FirstOrDefault();
            return mostFrequentGenre;
        }

        private async Task<string[]> GetGameIdsFromUserLibraryAsync(Guid userId, string jwt)
        {
            try
            {
                var httpClient = httpClientFactory.CreateClient("UsersApi");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                var libraryResponse = await httpClient.GetAsync($"api/UserGameLibrary/{userId}");

                if (!libraryResponse.IsSuccessStatusCode)
                    throw new BusinessErrorDetailsException("Não foi possível buscar a biblioteca do usuário");

                var json = await libraryResponse.Content.ReadAsStringAsync();

                // Parse genérico sem DTO
                using var doc = JsonDocument.Parse(json);

                var root = doc.RootElement;

                // navegar até "data" e extrair todos os "gameId"
                var gameIds = root
                    .GetProperty("data")
                    .EnumerateArray()
                    .Select(x => x.GetProperty("gameId").GetString()!)
                    .ToArray();

                return gameIds;
            }
            catch (Exception ex) {
                throw new BusinessErrorDetailsException("Algo deu erro");
            }
        }
    }
}
