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
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();
        services.AddScoped<IGameRepository, ElasticsearchGameRepository>();
        services.AddScoped<IGameRecommendationService, RecommendationService>();
        services.AddScoped<IUserLibraryClient, UserLibraryClient>();
        services.AddScoped<IPromotionService, PromotionService>();
    }

    public static void ConfigureHttpClients(this IServiceCollection services, IConfigurationSection apiSection)
    {
        services.AddHttpClient("UsersApi", client =>
        {
            client.BaseAddress = new Uri(apiSection["UsersApiBaseUrl"] ?? "");
        });
    }
}
