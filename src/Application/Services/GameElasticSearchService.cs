using Application.DTOs;
using Application.DTOs.Requests;
using Application.DTOs.Response;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class GameElasticSearchService : IGameElasticSearchService
    {
        private readonly ElasticsearchClient _client;

        private readonly IMapper _mapper;

        private readonly IConfiguration _configuration;

        public GameElasticSearchService(
            ElasticsearchClient client, IMapper mapper, IConfiguration configuration)
        {
            _client = client;
            _mapper = mapper;
            _configuration = configuration;
        }


        /***
         * Deletar todos os jogos do elastic search
         */
        public async Task<long> DeleteAllAsync(CancellationToken ct = default)
        {
            var index = "search-97tr";

            // Deleta TODOS os docs do índice
            var response = await _client.DeleteByQueryAsync(index, d => d
                .Query(q => q.MatchAll(new MatchAllQuery()))
                .Conflicts(Conflicts.Proceed)   // ignora conflitos de versão
                .Refresh(true)                  // força refresh ao final
                .WaitForCompletion(true),       // aguarda terminar
                ct);

            if (!response.IsValidResponse)
            {
                // Você pode trocar por ILogger
                var detail = response.ElasticsearchServerError?.ToString() ?? response.DebugInformation;
                throw new InvalidOperationException($"Falha ao apagar documentos do índice '{index}': {detail}");
            }

            // Em algumas versões, Deleted pode ser null; trate como 0
            return response.Deleted ?? 0L;
        }


        /***
         * Adcionar novo jogo ao elastic search
         */
        public async Task<string> IndexGameAsync(Game game, CancellationToken ct = default)
        {
            var index = "search-97tr";

            if (game.CreatedAt == default)
                game.CreatedAt = DateTime.UtcNow;

            var esId = Guid.NewGuid().ToString("N"); // <- _id único

            var response = await _client.IndexAsync(game, i => i
                .Index(index)
                .Id(esId)                  // <- evita a inferência do Game.Id = 0
                .OpType(Elastic.Clients.Elasticsearch.OpType.Create), // falha se _id já existir
                ct);

            if (!response.IsValidResponse)
            {
                var detail = response.ElasticsearchServerError?.ToString() ?? response.DebugInformation;
                throw new InvalidOperationException($"Falha ao indexar jogo em '{index}': {detail}");
            }

            return response.Id; 
        }

        public async Task<Game?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var index = "search-97tr";

            var get = await _client.GetAsync<Game>(id, g => g.Index(index), ct);
            if (!get.IsValidResponse || !get.Found) return null;
            return get.Source;
        }

        public async Task<bool> DeleteByIdAsync(string id, CancellationToken ct = default)
        {
            var index = "search-97tr";

            //var del = await _client.DeleteAsync<Game>(id, d => d.Index(index).Refresh(true), ct);
            var del = await _client.DeleteAsync<Game>(id, d => d.Index(index), ct);

            if (del.ApiCallDetails?.HttpStatusCode == 404) return false;

            if (!del.IsValidResponse)
            {
                var detail = del.ElasticsearchServerError?.ToString() ?? del.DebugInformation;
                throw new InvalidOperationException($"Falha ao deletar jogo '{id}' em '{index}': {detail}");
            }

            return del.Result == Elastic.Clients.Elasticsearch.Result.Deleted;
        }
    }
}
