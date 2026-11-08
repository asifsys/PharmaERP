using System.Diagnostics;
using Microsoft.Extensions.Options;
using PharmaERP.Dashboard.API.Configuration;
using PharmaERP.Dashboard.API.DTOs;
using PharmaERP.Dashboard.API.Interfaces;

namespace PharmaERP.Dashboard.API.Repositories;

public sealed class ServiceHealthRepository
    : IServiceHealthRepository
{
    private readonly HttpClient _httpClient;

    private readonly ServiceRegistryOptions _options;

    private readonly ILogger<ServiceHealthRepository> _logger;

    public ServiceHealthRepository(
        HttpClient httpClient,
        IOptions<ServiceRegistryOptions> options,
        ILogger<ServiceHealthRepository> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<List<ServiceStatusDto>> GetServiceStatusesAsync(
        CancellationToken cancellationToken)
    {
        var tasks = _options.Services.Select(
            service => CheckServiceAsync(
                service, cancellationToken));

        // Query all registered services concurrently.
        var results = await Task.WhenAll(tasks);

        return results.ToList();
    }

    private async Task<ServiceStatusDto> CheckServiceAsync(
        ServiceEndpoint service,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var response = await _httpClient.GetAsync(
                service.HealthUrl,
                cancellationToken);

            stopwatch.Stop();

            return new ServiceStatusDto
            {
                ServiceName = service.Name,
                Department = service.Department,
                Status = response.IsSuccessStatusCode
                    ? "Healthy"
                    : "Degraded",
                ResponseTimeMs = stopwatch.Elapsed.TotalMilliseconds,
                LastChecked = DateTime.UtcNow
            };
        }
        catch (Exception ex) when (
            ex is HttpRequestException or TaskCanceledException)
        {
            stopwatch.Stop();

            _logger.LogWarning(
                "Health check failed for {Service}: {Error}",
                service.Name,
                ex.Message);

            return new ServiceStatusDto
            {
                ServiceName = service.Name,
                Department = service.Department,
                Status = "Offline",
                ResponseTimeMs = stopwatch.Elapsed.TotalMilliseconds,
                LastChecked = DateTime.UtcNow
            };
        }
    }
}