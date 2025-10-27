using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.MGet;
using Elastic.Clients.Elasticsearch.QueryDsl;
using FCG.Games.Domain.Entities;
using FCG.Games.Domain.Interfaces;

namespace FCG.Games.Infra.Data.Repository;

public class ElasticsearchGameRepository(ElasticsearchClient client) : IGameRepository
{
    private const string INDEX = "search-97tr";

    public async Task AddAsync(Game game)
    {
        var response = await client.IndexAsync(game, i => i
            .Index(INDEX)
            .Id(game.Id)
            .OpType(OpType.Create));

        if (!response.IsValidResponse)
        {
            var errorMessage = response.ElasticsearchServerError?.ToString() ?? response.DebugInformation;
            throw new InvalidOperationException($"Falha ao indexar jogo em '{INDEX}': {errorMessage}");
        }
    }
    public async Task<Game?> GetByIdAsync(string id)
    {
        var response = await client.GetAsync<Game>(id, g => g.Index(INDEX));
        
        if (!response.IsValidResponse || response is null)
            return null;

        return Game.Reconstruct(
            response.Id,
            response.Source!.Title,
            response.Source.Price,
            response.Source.Description,
            response.Source.Genre,
            response.Source.Popularity
        );
    }
    public async Task<IEnumerable<Game?>> GetGamesByIdsAsync(List<string> ids)
    {
        var response = await client.MultiGetAsync<Game>(m => m
        .Index(INDEX)
        .Ids(ids.ToArray())
    );

        if (!response.IsValidResponse)
            throw new Exception("GetGamesByIdsAsync falhou");

        return response.Docs
            .OfType<MultiGetResponseItem<Game>>()
            .Select(doc => doc.Match(
                result => result.Source, _ => null))
            .Where(game => game != null)
            .ToList();
    }
    public async Task<List<Game>> GetGamesByGenreAsync(string genre, int page, int size, List<string>? excludeIds = null)
    {
        var mustNotQueries = new List<Action<QueryDescriptor<Game>>>();

        if (excludeIds is not null)
            mustNotQueries.Add(mn => mn.Ids(i => i.Values(excludeIds.ToArray())));

        var response = await client.SearchAsync<Game>(s => s
            .Indices(INDEX)
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

        return response.Documents.ToList();
    }
    public async Task<IEnumerable<Game>> GetPaginatedAsync(int page, int size)
    {
        var response = await client.SearchAsync<Game>(s => s
                .Indices(INDEX)
                .From((page - 1) * size)
                .Size(size)
            );
        if (!response.IsValidResponse)
            throw new Exception("Erro ao buscar jogos paginados");

        var hitDictionary = response.Hits
            .ToDictionary(h => h.Source!, h => h.Id);

        return response.Documents
            .Select(doc => Game.Reconstruct(
                hitDictionary[doc],
                doc.Title,
                doc.Price,
                doc.Description,
                doc.Genre,
                doc.Popularity
            )
        ).ToList();
    }
    public async Task<IEnumerable<Game>> GetMostPopularPaginatedAsync(int page, int size)
    {
        var response = await client.SearchAsync<Game>(s => s
                .Indices(INDEX)
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
            .Select(doc => Game.Reconstruct(
                hitDictionary[doc],
                doc.Title,
                doc.Price,
                doc.Description,
                doc.Genre,
                doc.Popularity
            )
        ).ToList();
    }
    public async Task<bool> TitleExistsAsync(string title)
    {
        var response = await client.SearchAsync<Game>(s => s
            .Indices(INDEX)
            .Size(0)
            .Query(q => q
               .Match(m => m.Field(f => f.Title).Query(title))
            )
        );

        return response.Total > 0;
    }
    public async Task<bool> TitleExistsExcludingIdAsync(string title, string excludeId)
    {
        var response = await client.SearchAsync<Game>(s => s
            .Indices(INDEX)
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
        var response = await client.UpdateAsync<Game, Game>(
            INDEX,
            id,
            u => u.Doc(game)
        );

        if (!response.IsValidResponse) throw new Exception("Update falhou");
    }
    public async Task DeleteByIdAsync(string id)
    {
        var response = await client.DeleteAsync(INDEX, id);
    
        if (!response.IsValidResponse)
        {
            if (response.Result == Result.NotFound)
                throw new Exception($"Jogo com ID '{id}' não encontrado");

            throw new Exception($"Erro ao deletar jogo: {response.ElasticsearchServerError?.Error?.Reason}");
        }
    }
    public async Task DeleteAllAsync()
    {
        var response = await client.DeleteByQueryAsync(INDEX, d => d
            .Query(q => q.MatchAll(new MatchAllQuery()))
            .Conflicts(Conflicts.Proceed)
            .Refresh(true)
            .WaitForCompletion(true)
        );

        if (!response.IsValidResponse)
        {
            var errorMessage = response.ElasticsearchServerError?.ToString() ?? response.DebugInformation;
            throw new InvalidOperationException($"Falha ao apagar documentos do índice '{INDEX}': {errorMessage}");
        }
    }
}
