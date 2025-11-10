using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.MGet;
using Elastic.Clients.Elasticsearch.QueryDsl;
using FCG.Games.Domain.Entities;
using FCG.Games.Domain.Interfaces.Repository;
using FCG.Games.Infra.Data.Documents;
using Microsoft.Extensions.Configuration;

namespace FCG.Games.Infra.Data.Repository;

public class ElasticsearchGameRepository : IGameRepository
{
    private readonly ElasticsearchClient _client;
    private readonly string _indexName;

    public ElasticsearchGameRepository(ElasticsearchClient client, IConfiguration configuration)
    {
        _client = client;
        _indexName = configuration["ElasticSearch:IndexName"] ?? "games";
    }

    public async Task AddAsync(Game game)
    {
        var response = await _client.IndexAsync(game, i => i
            .Index(_indexName)
            .Id(game.Id)
            .OpType(OpType.Create)
        );

        if (!response.IsValidResponse)
        {
            var errorMessage = response.ElasticsearchServerError?.ToString() ?? response.DebugInformation;
            throw new InvalidOperationException($"Falha ao indexar jogo em '{_indexName}': {errorMessage}");
        }
    }
    public async Task<Game?> GetByIdAsync(string id)
    {
        var response = await _client.GetAsync<GameDocument>(id, g => g.Index(_indexName));
        
        if (!response.IsValidResponse || response is null)
            return null;

        return response.Source!.ToEntity(response.Id);
    }
    public async Task<IEnumerable<Game?>> GetGamesByIdsAsync(List<string> ids)
    {
        var response = await _client.MultiGetAsync<GameDocument>(m => m
        .Index(_indexName)
        .Ids(ids.ToArray())
    );

        if (!response.IsValidResponse)
            throw new Exception("GetGamesByIdsAsync falhou");

        return response.Docs
            .OfType<MultiGetResponseItem<GameDocument>>()
            .Select(doc => doc.Match(
                result => result.Source.ToEntity(result.Id), _ => null))
            .Where(game => game != null)
            .ToList();
    }
    public async Task<List<Game>> GetGamesByGenreAsync(string genre, int page, int size, List<string>? excludeIds = null)
    {
        var mustNotQueries = new List<Action<QueryDescriptor<GameDocument>>>();

        if (excludeIds is not null)
            mustNotQueries.Add(mn => mn.Ids(i => i.Values(excludeIds.ToArray())));

        var response = await _client.SearchAsync<GameDocument>(s => s
            .Indices(_indexName)
            .From((page - 1) * size)
            .Size(size)
            .Query(q => q.Bool(b =>
            {
                b.Must(m => m.Term(t => t
                    .Field("genre.keyword")
                    .Value(genre)
                    .CaseInsensitive(true)
                ));

                if (mustNotQueries.Any())
                    b.MustNot(mustNotQueries.ToArray());
            }))
            .Sort(sort => sort.Field(g => g.Popularity, SortOrder.Desc))
        );

        if (!response.IsValidResponse)
            throw new Exception("GetGamesByGenreAsync falhou");

        var hitDictionary = response.Hits
            .ToDictionary(h => h.Source!, h => h.Id);

        return response.Documents
        .Select(doc => doc.ToEntity(hitDictionary[doc]))
        .ToList();
    }
    public async Task<IEnumerable<Game>> GetPaginatedAsync(int page, int size)
    {
        var response = await _client.SearchAsync<GameDocument>(s => s
                .Indices(_indexName)
                .From((page - 1) * size)
                .Size(size)
            );
        if (!response.IsValidResponse)
            throw new Exception("Erro ao buscar jogos paginados");

        var hitDictionary = response.Hits
            .ToDictionary(h => h.Source!, h => h.Id);

        return response.Documents
            .Select(doc => doc.ToEntity(hitDictionary[doc]))
            .ToList();
    }
    public async Task<IEnumerable<Game>> GetMostPopularPaginatedAsync(int page, int size)
    {
        var response = await _client.SearchAsync<GameDocument>(s => s
                .Indices(_indexName)
                .From((page - 1) * size)
                .Size(size)
                .Sort(sort => sort
                    .Field(g => g.Popularity, SortOrder.Desc)
                )
            );
        if (!response.IsValidResponse)
            throw new Exception("Erro ao buscar jogos mais populares");

        var hitDictionary = response.Hits
            .ToDictionary(h => h.Source!, h => h.Id);

        return response.Documents
            .Select(doc => doc.ToEntity(hitDictionary[doc]))
            .ToList();
    }
    public async Task<bool> TitleExistsAsync(string title)
    {
        var response = await _client.SearchAsync<GameDocument>(s => s
            .Indices(_indexName)
            .Size(0)
            .Query(q => q
               .Match(m => m.Field(f => f.Title).Query(title))
            )
        );

        return response.Total > 0;
    }
    public async Task<bool> TitleExistsExcludingIdAsync(string title, string excludeId)
    {
        var response = await _client.SearchAsync<GameDocument>(s => s
            .Indices(_indexName)
            .Size(0)
            .Query(q => q
                .Bool(b => b
                    .Must(m => m
                        .Match(ma => ma
                            .Field(f => f.Title)
                            .Query(title)
                        )
                    )
                    .MustNot(mn => mn
                        .Ids(i => i.Values(new[] { excludeId }))
                    )
                )
            )
        );

        return response.IsValidResponse && response.Total > 0;
    }
    public async Task UpdateAsync(string id, Game game)
    {
        var response = await _client.UpdateAsync<Game, Game>(
            _indexName,
            id,
            u => u.Doc(game)
        );

        if (!response.IsValidResponse) throw new Exception("Update falhou");
    }
    public async Task DeleteByIdAsync(string id)
    {
        var response = await _client.DeleteAsync(_indexName, id);
    
        if (!response.IsValidResponse)
        {
            if (response.Result == Result.NotFound)
                throw new Exception($"Jogo com ID '{id}' não encontrado");

            throw new Exception($"Erro ao deletar jogo: {response.ElasticsearchServerError?.Error?.Reason}");
        }
    }
    public async Task DeleteAllAsync()
    {
        var response = await _client.DeleteByQueryAsync(_indexName, d => d
            .Query(q => q.MatchAll(new MatchAllQuery()))
            .Conflicts(Conflicts.Proceed)
            .Refresh(true)
            .WaitForCompletion(true)
        );

        if (!response.IsValidResponse)
        {
            var errorMessage = response.ElasticsearchServerError?.ToString() ?? response.DebugInformation;
            throw new InvalidOperationException($"Falha ao apagar documentos do índice '{_indexName}': {errorMessage}");
        }
    }
}
