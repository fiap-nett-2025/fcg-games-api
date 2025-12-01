using FCG.Games.Application;
using FCG.Games.Application.DTOs;
using FCG.Games.Application.Handler;
using FCG.Games.Domain.Interfaces.Messaging;
using FCG.Games.Infra;
using FCG.Games.Infra.Config;
using FCG.Games.Infra.Messaging;
using FCG.Games.Infra.Messaging.Config;
using FCG.Games.Infra.Persistence.Config;
using FCG.Games.Worker;

var builder = Host.CreateApplicationBuilder(args);

#region Elasticsearch
var elasticsearchSection = builder.Configuration.GetSection("ElasticSearch");
if (!elasticsearchSection.Exists())
    throw new InvalidOperationException("Section 'ElasticSearch' not found in configuration.");

builder.Services.Configure<ElasticsearchOptions>(elasticsearchSection);
builder.Services.ConfigureElasticsearch();
#endregion

#region RabbitMQ Configuration
var rabbitSection = builder.Configuration.GetSection("RabbitMq");

var rabbitSettingsSection = rabbitSection.GetSection("Settings");
if (!rabbitSettingsSection.Exists())
    throw new InvalidOperationException("Section 'RabbitMqSettings' not found in configuration");
builder.Services.Configure<RabbitMqOptions>(rabbitSettingsSection);

var exchangeSection = rabbitSection.GetSection("Exchanges");
if (!exchangeSection.Exists())
    throw new InvalidOperationException("Section 'Exchanges' not found in configuration.");
builder.Services.Configure<ExchangesOptions>(exchangeSection);

var queueSection = rabbitSection.GetSection("Queues");
if (!queueSection.Exists())
    throw new InvalidOperationException("Section 'Queues' not found in configuration.");
builder.Services.Configure<QueuesOptions>(queueSection);
builder.Services.ConfigureRabbitMq();
#endregion

builder.Services.ConfigureServices();
builder.Services.ConfigureHttpClients(builder.Configuration.GetSection("ClientAPI"));
builder.Services.AddScoped<IMessageHandler<MessageDTO>, GameIncreasePopularityHandler>();
builder.Services.AddScoped<IQueueConsumer<MessageDTO>, RabbitMqConsumer<MessageDTO>>();
builder.Services.AddHostedService<GamePopularityConsumerWorker>();

var host = builder.Build();
host.Run();
