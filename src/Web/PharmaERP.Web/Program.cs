

using PharmaERP.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Gateway URL should be configured per environment.
var gatewayUrl = builder.Configuration["Gateway:BaseUrl"]
    ?? throw new InvalidOperationException(
        "Gateway URL is missing.");

builder.Services.AddHttpClient<DashboardApiService>(client =>
{
    client.BaseAddress = new Uri(gatewayUrl);

    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
