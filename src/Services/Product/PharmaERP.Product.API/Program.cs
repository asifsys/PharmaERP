using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PharmaERP.Product.API.Data;
using PharmaERP.Product.API.Middleware;
using PharmaERP.Product.API.Repositories;
using PharmaERP.Product.API.Services;
using Microsoft.AspNetCore.Builder;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Add Health Checks here:
builder.Services.AddHealthChecks().AddDbContextCheck<ProductDbContext>("product-database");

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ProductDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ProductDb")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();

var authority = builder.Configuration["Authentication:Authority"];
var audience = builder.Configuration["Authentication:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.Audience = audience;
        options.RequireHttpsMetadata =  builder.Configuration.GetValue<bool>("Authentication:RequireHttpsMetadata");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType = "role",
            NameClaimType = "name"
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProductRead", policy =>
        policy.RequireRole("ProductAdmin", "ExecutiveAdmin", "Auditor", "DepartmentManager"));
    options.AddPolicy("ProductWrite", policy =>
        policy.RequireRole("ProductAdmin", "ExecutiveAdmin"));
});



builder.Services.AddCors(options =>
{
    options.AddPolicy("PharmaErpGateway", policy =>
        policy.WithOrigins("https://localhost:7200", "https://localhost:7300")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Point Swagger UI to Microsoft's native json file
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("PharmaErpGateway");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/ready");

app.Run();

public partial class Program { }
