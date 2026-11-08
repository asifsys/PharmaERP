using PharmaERP.Observability;
using PharmaERP.Dashboard.API.Configuration;
using PharmaERP.Dashboard.API.Interfaces;
using PharmaERP.Dashboard.API.Repositories;
using PharmaERP.Dashboard.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Register MVC API controllers.
builder.Services.AddControllers();

// Register application health checks.
builder.Services.AddHealthChecks();

// Register HTTP client support.
// Used later for communication with other microservices.
builder.Services.AddHttpClient();


// Configure Swagger.
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// Bind service registry configuration.
builder.Services.Configure<ServiceRegistryOptions>(
    builder.Configuration.GetSection("ServiceRegistry"));

// Configure health-check HTTP client.
// Each health check gets a maximum execution time of 5 seconds.
builder.Services.AddHttpClient<IServiceHealthRepository,
    ServiceHealthRepository>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(5);
    });

// Register application services.
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

// Enable Swagger during development.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
    // Map the OpenAPI endpoints in the middleware pipeline
    app.MapOpenApi();
    // Point Swagger UI to Microsoft's native json file
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API v1");
    });
}

// Add request tracing middleware.
app.UseMiddleware<CorrelationIdMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Health endpoint for monitoring and load balancers.
app.MapHealthChecks("/health");

// Readiness endpoint will receive additional dependency
// checks for SQL Server, Redis and RabbitMQ later.
app.MapGet("/ready", () =>
    Results.Ok(new
    {
        Status = "Ready",
        Service = "Dashboard",
        Timestamp = DateTime.UtcNow
    }));

app.Run();

// Allows integration tests to access the application entry point.
public partial class Program { }