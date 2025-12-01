using Elastic.Clients.Elasticsearch;
using FCG.Games.API.Configurations;
using FCG.Games.Application;
using FCG.Games.Application.Middlewares;
using FCG.Games.Infra;
using FCG.Games.Infra.Persistence.Config;
using FCG.Games.Infra.Persistence.Data;
using FCG.Games.Infra.Seedings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

internal class Program
{
    private static async Task Main(string[] args)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        #region Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSwaggerConfiguration();
        #endregion

        #region JWT Authentication
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
        #endregion

        #region Dependency Injection

        #region SQL Server
        var sqlServerSection = builder.Configuration.GetSection("SqlServerSettings");
        if (!sqlServerSection.Exists())
            throw new InvalidOperationException("Section 'SqlServerSettings' not found in configuration.");

        builder.Services.Configure<SqlServerOptions>(sqlServerSection);
        builder.Services.ConfigureSQLServer();
        #endregion

        #region API clients
        var apiSection = builder.Configuration.GetSection("ClientAPI");
        if (!apiSection.Exists())
            throw new InvalidOperationException("Section 'ClientAPI' not found in configuration.");

        builder.Services.ConfigureHttpClients(apiSection);
        #endregion

        #region Elasticsearch
        var elasticsearchSection = builder.Configuration.GetSection("ElasticSearch");
        if (!elasticsearchSection.Exists())
            throw new InvalidOperationException("Section 'ElasticSearch' not found in configuration.");

        builder.Services.Configure<ElasticsearchOptions>(elasticsearchSection);
        builder.Services.ConfigureElasticsearch();
        #endregion

        builder.Services.ConfigureServices();
        #endregion

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
    }
}

#region Swagger

#endregion
#region JWT Authentication

#endregion
#region Dependency Injection
#region SQL Server

#endregion
#region RabbitMq

#endregion
#region API clients

#endregion

#endregion
