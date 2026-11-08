using PharmaERP.Dashboard.API.DTOs;

namespace PharmaERP.Dashboard.API.Interfaces;

public interface IServiceHealthRepository
{
    Task<List<ServiceStatusDto>> GetServiceStatusesAsync(
        CancellationToken cancellationToken);
}