using PharmaERP.Dashboard.API.DTOs;

namespace PharmaERP.Dashboard.API.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(
        CancellationToken cancellationToken);

    Task<List<ServiceStatusDto>> GetServicesAsync(
        CancellationToken cancellationToken);

    Task<object> SearchServicesAsync(
        ServiceQueryDto query,
        CancellationToken cancellationToken);
}