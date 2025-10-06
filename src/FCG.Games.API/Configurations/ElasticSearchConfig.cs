//using Elasticsearch.Net;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Nodes;
using Elastic.Transport;

namespace FCG.Games.API.Configurations
{
    public static class ElasticSearchConfig
    {
        public static IServiceCollection AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            
            var settings = new ElasticsearchClientSettings(new Uri(configuration["ElasticSearch:CloudUrl"]))
                .Authentication(new ApiKey(configuration["ElasticSearch:ApiKey"]))
                .RequestTimeout(TimeSpan.FromSeconds(300));

            var client = new ElasticsearchClient(settings);

            services.AddSingleton(client);

            return services;
        }
    }
}
