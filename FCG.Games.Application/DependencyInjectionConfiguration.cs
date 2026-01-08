using FCG.Games.Application.DTOs;
using FCG.Games.Application.Handler;
using FCG.Games.Application.Interfaces;
using FCG.Games.Application.Services;
using FCG.Games.Domain.Interfaces.Messaging;
using FCG.Games.Domain.Interfaces.Repository;
using FCG.Games.Infra.Messaging;
using FCG.Games.Infra.Persistence.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Games.Application;

public static class DependencyInjectionConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<IGameService, GameService>();
        services.AddTransient<IPromotionRepository, PromotionRepository>();
        services.AddTransient<IGameRepository, ElasticsearchGameRepository>();
        services.AddTransient<IGameRecommendationService, RecommendationService>();
        services.AddTransient<IUserLibraryClient, UserLibraryClient>();
        services.AddTransient<IPromotionService, PromotionService>();
        services.AddTransient<IMessageHandler<MessageDTO>, GameIncreasePopularityHandler>();

    }

    public static void ConfigureHttpClients(this IServiceCollection services, IConfigurationSection apiSection)
    {
        services.AddHttpClient("UsersApi", client =>
        {
            client.BaseAddress = new Uri(apiSection["UsersApiBaseUrl"] ?? "");
        });
    }
}
