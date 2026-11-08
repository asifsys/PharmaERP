using System.Net.Http.Json;

namespace PharmaERP.Web.Services;

public sealed class DashboardApiService
{
    private readonly HttpClient _httpClient;

    public DashboardApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T?> GetAsync<T>(
        string endpoint,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            endpoint,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(
            cancellationToken: cancellationToken);
    }
}