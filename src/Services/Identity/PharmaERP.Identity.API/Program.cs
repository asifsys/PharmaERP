using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using PharmaERP.Identity.API.Configurations;
using PharmaERP.Identity.API.Data;
using PharmaERP.Identity.API.Entities;
using PharmaERP.Identity.API.Services;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("IdentityDb") ?? throw new InvalidOperationException("IdentityDb connection string is missing.");

// --------------------------------------------------
// 1. Database
// --------------------------------------------------

builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseSqlServer(connectionString);

    options.UseOpenIddict();
});

// --------------------------------------------------
// 2. ASP.NET Core Identity
// --------------------------------------------------

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 12;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();

// --------------------------------------------------
// 3. OpenIddict
// --------------------------------------------------

var issuer = builder.Configuration["Identity:Issuer"]
    ?? throw new InvalidOperationException("Issuer missing.");

builder.Services.AddOpenIddict()

    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<IdentityDbContext>();
    })

    .AddServer(options =>
    {
        options.SetIssuer(new Uri(issuer));

        options.SetTokenEndpointUris("/connect/token");

        options.AllowClientCredentialsFlow();

        options.RegisterScopes(
            Scopes.Profile,
            Scopes.Email,
            Scopes.Roles,
            "pharmaerp.api");

        // Development-only certificates.
        // Replace with managed certificates in production.
        if (builder.Environment.IsDevelopment())
        {
            options.AddDevelopmentSigningCertificate();
            options.AddDevelopmentEncryptionCertificate();
        }

        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough();
    })

    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

// --------------------------------------------------
// 4. Application services
// --------------------------------------------------

builder.Services.AddScoped<IIdentityService, IdentityService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// --------------------------------------------------
// 5. Build application
// --------------------------------------------------

var app = builder.Build();

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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed initial roles.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await IdentitySeeder.SeedAsync(services);
}

app.Run();

public partial class Program { }