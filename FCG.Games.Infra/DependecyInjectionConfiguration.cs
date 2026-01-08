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
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using FCG.Games.Infra.Messaging.Sqs;

namespace FCG.Games.Infra;

public static class DependecyInjectionConfiguration
{
    public static void ConfigureSQLServer(this IServiceCollection services)
    {
        services.AddDbContext<FcgGameDbContext>((sp, options) =>
        {
            var settings = sp.GetRequiredService<IOptions<SqlServerOptions>>().Value;

            options.UseSqlServer(settings.GameConnection);
        }, ServiceLifetime.Scoped); 

    }

    //RabbitMq not used
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

    //Amazon SQS
    public static IServiceCollection ConfigureAmazonSQS(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonSQS>();

        services.AddTransient<IQueueConsumer, AmazonSqsConsumer>();

        return services;
    }
}