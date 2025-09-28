using FCG.Game.Application.AutoMapper;

namespace FCG.Game.API.Configurations
{
    public static class AutoMapperConfig
    {
        public static void AddAutoMapperConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<GameProfile>();
            });
        }
    }
}
