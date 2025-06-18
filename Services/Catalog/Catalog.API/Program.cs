using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddCarter();

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();

// Note: InitializeMartenWith moved to runtime for better error handling

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapCarter();

app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

// Initialize database in development
if (app.Environment.IsDevelopment())
{
    await InitializeDatabaseAsync(app);
}

app.Run();

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
            logger.LogInformation("🔄 Attempting to connect to Catalog PostgreSQL (attempt {Retry}/{MaxRetries})", retry + 1, maxRetries);

            // Initialize Marten with seed data
            var documentStore = scope.ServiceProvider.GetRequiredService<IDocumentStore>();

            // Ensure database exists and apply schema
            await documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync();

            // Seed data
            var seedData = new CatalogInitialData();
            await seedData.Populate(documentStore, CancellationToken.None);

            logger.LogInformation("✅ Catalog database initialization completed successfully");
            return;
        }
        catch (Exception ex)
        {
            logger.LogWarning("⚠️ Catalog database connection failed (attempt {Retry}/{MaxRetries}): {Error}",
                retry + 1, maxRetries, ex.Message);

            if (retry == maxRetries - 1)
            {
                logger.LogError("❌ Failed to connect to Catalog PostgreSQL after {MaxRetries} attempts", maxRetries);
                throw;
            }

            await Task.Delay(retryDelay);
        }
    }
}
