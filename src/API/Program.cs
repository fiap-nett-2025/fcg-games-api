using API.Configurations;
using Application.Middlewares;
using Application.Services;
using Application.Interfaces;
using Domain.Interfaces;
using Infra.Data;
using Infra.Repository;
using Infra.Seedings;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.Text;
using Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ Connection String
builder.Services.AddDbContext<GameDbContext>(options =>
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

// ✅ Serviços
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddHttpClient();

builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = new Uri("https://user-api:8080/");
});

builder.Services.AddHttpClient<IPaymentService, PaymentService>(client =>
{
    client.BaseAddress = new Uri("http://payment-api:8080/");
});

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
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        if (dbContext.Database.EnsureCreated())
        {
            await GameSeeding.SeedAsync(dbContext);
        }
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
