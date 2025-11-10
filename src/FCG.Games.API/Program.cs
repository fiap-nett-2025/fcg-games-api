using Elastic.Clients.Elasticsearch;
using FCG.Games.API.Configurations;
using FCG.Games.Application.Interfaces;
using FCG.Games.Application.Middlewares;
using FCG.Games.Application.Services;
using FCG.Games.Domain.Interfaces;
using FCG.Games.Domain.Interfaces.Repository;
using FCG.Games.Domain.Services;
using FCG.Games.Infra.Data.Data;
using FCG.Games.Infra.Data.Repository;
using FCG.Games.Infra.Data.Seedings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Connection String
builder.Services.AddDbContext<FcgGameDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)
    );
}, ServiceLifetime.Scoped);


// ✅ JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var chaveSecreta = jwtSettings["Key"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("Authenticated", policy =>
        policy.RequireAuthenticatedUser());
});

#region API clients
var apiSection = builder.Configuration.GetSection("API");
if (!apiSection.Exists())
    throw new InvalidOperationException("Section 'API' not found in configuration.");

builder.Services.AddHttpClient("UsersApi", client =>
{
    client.BaseAddress = new Uri(apiSection["UsersApiBaseUrl"] ?? "");
});
#endregion


// ✅ Serviços
builder.Services.AddScoped<IGameService, GameService>();

builder.Services.AddElasticsearch(builder.Configuration);
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddSingleton(builder.Configuration);

builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<IGameRepository, ElasticsearchGameRepository>();
builder.Services.AddScoped<IGameRecommendationService, RecommendationService>();
builder.Services.AddScoped<IUserLibraryClient, UserLibraryClient>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddHttpClient();

builder.Services.AddSwaggerConfiguration();

// ✅ Swagger
builder.Services.AddEndpointsApiExplorer();

// ✅ Controllers
builder.Services.AddControllers();

builder.Logging.AddJsonConsole();

var app = builder.Build();

// ✅ Pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseSwaggerConfiguration();
    try
    {
        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FcgGameDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Aplicando migrações pendentes do banco de dados...");
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("Migrações aplicadas com sucesso.");

        var elasticClient = scope.ServiceProvider.GetRequiredService<ElasticsearchClient>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        await ElasticSearchConfig.InitializeElasticsearchIndexAsync(elasticClient, "fcg-games");
        await GameSeeding.SeedAsync(elasticClient, configuration);
        await PromotionSeeding.SeedAsync(dbContext);

        logger.LogInformation("Banco de dados criado e inicializado com sucesso.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao criar banco de dados: {ex.Message}");
    }
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseDeveloperExceptionPage();
app.UseExceptionHandler("/Error");
app.UseHsts();

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
