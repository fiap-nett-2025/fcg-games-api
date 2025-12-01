using Elastic.Clients.Elasticsearch;
using FCG.Games.Domain.Interfaces.Messaging;
using FCG.Games.Infra.Messaging;
using FCG.Games.Infra.Messaging.Config;
using FCG.Games.Infra.Persistence.Config;
using FCG.Games.Infra.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FCG.Games.Infra;

public static class DependecyInjectionConfiguration
{
    public static void ConfigureSQLServer(this IServiceCollection services)
    {
        services.AddDbContext<FcgGameDbContext>((sp, options) =>
        {
            var settings = sp.GetRequiredService<IOptions<SqlServerOptions>>().Value;

            options.UseSqlServer(
                settings.ConnectionStrings,
                sqlOptions =>
                {
                    if (settings.EnableRetryOnFailure)
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: settings.MaxRetryCount,
                            maxRetryDelay: TimeSpan.FromSeconds(settings.MaxRetryDelaySeconds),
                            errorNumbersToAdd: null);
                    }
                });
        }, ServiceLifetime.Scoped);
    }

    public static void ConfigureRabbitMq(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

            var factory = new ConnectionFactory
            {
                HostName = settings.HostName,
                Port = settings.Port,
                VirtualHost = settings.VirtualHost,
                UserName = settings.UserName,
                Password = settings.Password,
                ClientProvidedName = settings.ClientProvidedName,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            if (settings.UseSsl)
            {
                factory.Ssl = new SslOption
                {
                    Enabled = true,
                    ServerName = settings.HostName
                };
            }

            return factory;
        });
    }

    public static void ConfigureElasticsearch(this IServiceCollection services)
    {
        services.AddSingleton<ElasticsearchClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;
            
            var connectionSettings = new ElasticsearchClientSettings(new Uri(settings.Url))
                .DefaultIndex(settings.IndexName);
            return new ElasticsearchClient(connectionSettings);
        });
    }
}