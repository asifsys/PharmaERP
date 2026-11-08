namespace PharmaERP.Dashboard.API.DTOs;

public sealed class DashboardSummaryDto
{
    public int TotalServices { get; set; }

    public int HealthyServices { get; set; }

    public int DegradedServices { get; set; }

    public int OfflineServices { get; set; }
}