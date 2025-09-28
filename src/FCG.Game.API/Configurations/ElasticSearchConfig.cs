//using Elasticsearch.Net;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Nodes;
using Elastic.Transport;

namespace FCG.Game.API.Configurations
{
    public static class ElasticSearchConfig
    {
        public static IServiceCollection AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            /*var url = "https://my-elasticsearch-project-aa4997.es.us-central1.gcp.elastic.cloud:443";

            var user = "elastic";

            var apiKey = "bkJOeWg1a0JQWnNUWEN6NkYxSUg6R0ZNQmU4bjJHYTRIT0Zsb0l1SGlHUQ==";

            var indexDefault = "games";*/

            var settings = new ElasticsearchClientSettings(new Uri("https://my-elasticsearch-project-cf1489.es.us-central1.gcp.elastic.cloud:443"))
                .Authentication(new ApiKey("N2k2cWlKa0JMVXFaYWM0cDFPYkk6VUF1dS1NVjMySWY3YklVYnJNb2E4Zw=="))
                .RequestTimeout(TimeSpan.FromSeconds(300));

            var client = new ElasticsearchClient(settings);

            services.AddSingleton(client);

            return services;
        }
    }
}
