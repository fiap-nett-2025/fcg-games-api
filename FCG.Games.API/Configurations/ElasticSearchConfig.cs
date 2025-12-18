using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using FCG.Games.Domain.Entities;

namespace FCG.Games.API.Configurations
{
    public static class ElasticSearchConfig
    {
        public static IServiceCollection AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["ElasticSearch:Url"];
            var apiKey = configuration["ElasticSearch:ApiKey"];

            ElasticsearchClientSettings settings;
            if(!string.IsNullOrEmpty(apiKey))
            {
                settings = new ElasticsearchClientSettings(new Uri(url!))
                    .Authentication(new ApiKey(apiKey))
                    .RequestTimeout(TimeSpan.FromSeconds(300));
            }
            else
            {
                settings = new ElasticsearchClientSettings(new Uri(url!))
                    .RequestTimeout(TimeSpan.FromSeconds(300));
            }

            var client = new ElasticsearchClient(settings);
            services.AddSingleton(client);

            return services;
        }

        public static async Task InitializeElasticsearchIndexAsync(ElasticsearchClient client, string indexName)
        {
            var existResponse = await client.Indices.ExistsAsync(indexName);
            if(!existResponse.Exists)
            {
                var createResponse = await client.Indices.CreateAsync(indexName, c => c
                    .Mappings(m => m
                        .Properties<Game>(p => p
                            .Keyword(k => k.Id)
                            .Text(t => t.Title, td => td.Fields(f => f.Keyword("keyword")))
                            .FloatNumber(f => f.Price)
                            .Text(t => t.Description)
                            .Keyword(k => k.Genre)
                            .IntegerNumber(i => i.Popularity)
                        )
                    )
                );

                if(!createResponse.IsValidResponse)
                    throw new Exception($"Falha ao criar Índice '{indexName}': {createResponse.DebugInformation}");
            }
        }
    }
}
