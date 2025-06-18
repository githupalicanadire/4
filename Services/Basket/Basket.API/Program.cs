using Discount.Grpc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Caching.Distributed;
using BuildingBlocks.Messaging.MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Application Services
var assembly = typeof(Program).Assembly;
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

//Data Services
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
    opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    //options.InstanceName = "Basket";
});

//Grpc Services
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return handler;
});

//Async Communication Services
builder.Services.AddMessageBroker(builder.Configuration);

//Authentication & Authorization
// Clear default claim mappings to preserve original JWT claims
Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://localhost:6007";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:6007",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            NameClaimType = "preferred_username", // Map username claim
            RoleClaimType = "role" // Map role claim
        };

        // Preserve original claim names
        options.MapInboundClaims = false;
    });

builder.Services.AddAuthorization();

//CORS for React app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowShoppingApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:6006")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

//Cross-Cutting Services
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();

// Initialize database in development (no seed data for baskets - users create their own)
if (app.Environment.IsDevelopment())
{
    await InitializeDatabaseAsync(app);
}

async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    // Retry logic for database connection
    var maxRetries = 30;
    var retryDelay = TimeSpan.FromSeconds(2);

    for (int retry = 0; retry < maxRetries; retry++)
    {
        try
        {
            logger.LogInformation("🔄 Attempting to connect to PostgreSQL (attempt {Retry}/{MaxRetries})", retry + 1, maxRetries);

            // Test Marten connection
            var documentStore = scope.ServiceProvider.GetRequiredService<IDocumentStore>();
            using var session = documentStore.LightweightSession();

            // Ensure database exists and is migrated
            await documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync();

            // Test Redis connection
            logger.LogInformation("🔄 Testing Redis connection...");
            var distributedCache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
            await distributedCache.SetStringAsync("test-key", "test-value");
            var testValue = await distributedCache.GetStringAsync("test-key");
            await distributedCache.RemoveAsync("test-key");

            if (testValue == "test-value")
            {
                logger.LogInformation("✅ Redis connection successful");
            }
            else
            {
                logger.LogWarning("⚠️ Redis connection issue - cache may not work properly");
            }

            logger.LogInformation("✅ Basket service initialization completed successfully");
            logger.LogInformation("ℹ️ Baskets use Redis for caching and PostgreSQL for persistence");
            logger.LogInformation("ℹ️ Users will create their own shopping carts - no pre-seeded baskets");
            return;
        }
        catch (Exception ex)
        {
            logger.LogWarning("⚠️ Database connection failed (attempt {Retry}/{MaxRetries}): {Error}",
                retry + 1, maxRetries, ex.Message);

            if (retry == maxRetries - 1)
            {
                logger.LogError("❌ Failed to connect to PostgreSQL after {MaxRetries} attempts", maxRetries);
                throw;
            }

            await Task.Delay(retryDelay);
        }
    }
}

// Configure the HTTP request pipeline.
app.UseCors("AllowShoppingApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapCarter();
app.UseExceptionHandler(options => { });
app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();
