using PharmaERP.Dashboard.API.DTOs;
using PharmaERP.Dashboard.API.Interfaces;

namespace PharmaERP.Dashboard.API.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly IServiceHealthRepository _repository;

    public DashboardService(IServiceHealthRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ServiceStatusDto>> GetServicesAsync(
        CancellationToken cancellationToken)
    {
        return await _repository.GetServiceStatusesAsync(
            cancellationToken);
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(
        CancellationToken cancellationToken)
    {
        var services = await GetServicesAsync(cancellationToken);

        return new DashboardSummaryDto
        {
            TotalServices = services.Count,

            HealthyServices = services.Count(
                x => x.Status == "Healthy"),

            DegradedServices = services.Count(
                x => x.Status == "Degraded"),

            OfflineServices = services.Count(
                x => x.Status == "Offline")
        };
    }

    public async Task<object> SearchServicesAsync(
        ServiceQueryDto query,
        CancellationToken cancellationToken)
    {
        var services = await GetServicesAsync(cancellationToken);

        // Search across service name and department.
        var filtered = services.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            filtered = filtered.Where(x =>
                x.ServiceName.Contains(
                    query.Search,
                    StringComparison.OrdinalIgnoreCase) ||
                x.Department.Contains(
                    query.Search,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.Department))
        {
            filtered = filtered.Where(x =>
                x.Department.Equals(
                    query.Department,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            filtered = filtered.Where(x =>
                x.Status.Equals(
                    query.Status,
                    StringComparison.OrdinalIgnoreCase));
        }

        // Allow only predefined sort columns.
        filtered = (query.SortColumn.ToLowerInvariant(),
                    query.SortDirection.ToLowerInvariant()) switch
        {
            ("department", "desc") =>
                filtered.OrderByDescending(x => x.Department),

            ("department", _) =>
                filtered.OrderBy(x => x.Department),

            ("status", "desc") =>
                filtered.OrderByDescending(x => x.Status),

            ("status", _) =>
                filtered.OrderBy(x => x.Status),

            ("responsetimems", "desc") =>
                filtered.OrderByDescending(x => x.ResponseTimeMs),

            ("responsetimems", _) =>
                filtered.OrderBy(x => x.ResponseTimeMs),

            ("servicename", "desc") =>
                filtered.OrderByDescending(x => x.ServiceName),

            _ => filtered.OrderBy(x => x.ServiceName)
        };

        var totalRecords = filtered.Count();

        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var data = filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new
        {
            Data = data,
            TotalRecords = totalRecords,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(
                totalRecords / (double)pageSize)
        };
    }
}