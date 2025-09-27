using Application.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace API.Configurations
{
    public static class AutoMapperConfig
    {
        public static void AddAutoMapperConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<DomainToDtoGameES>();
                cfg.AddProfile<GameDtoProfile>();
                cfg.AddProfile<GameToSearchDocProfile>();
            });
        }
    }
}
