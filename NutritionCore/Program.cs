using System.Text;
using Microsoft.IdentityModel.Tokens;
using NutritionCore.Data;
using NutritionCore.DTO;
using NutritionCore.Services;
using NutritionCore.Services.Caching;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStackExchangeRedisCache(option =>
{
    option.Configuration = builder.Configuration.GetConnectionString("Redis");
    option.InstanceName = "Meals_";
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("24enbSecretKey24enbSecretKey24enbSecretKey"))
        };
    });

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddScoped<IMealService, MealService>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

builder.Services.AddScoped<ProducerService>();
builder.Services.AddScoped<ConsumerService>();
builder.Services.AddHostedService<ConsumerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ❶ Middleware, который собирает HTTP‑метрики (длительность, кол‑во, коды ответа)
app.UseHttpMetrics();

app.UseAuthentication();
app.UseAuthorization();

// ❷ Эндпоинт /metrics для Prometheus
app.MapMetrics();

app.MapControllers();

app.Run();