var builder = WebApplication.CreateBuilder(args);

// Load reverse proxy configuration.
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();

// Route incoming requests to microservices.
app.MapReverseProxy();

app.Run();