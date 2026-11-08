namespace PharmaERP.Web.Models;

public sealed class DashboardSummaryViewModel
{
    public int TotalServices { get; set; }

    public int HealthyServices { get; set; }

    public int DegradedServices { get; set; }

    public int OfflineServices { get; set; }
}

public sealed class ServiceStatusViewModel
{
    public string ServiceName { get; set; } = "";

    public string Department { get; set; } = "";

    public string Status { get; set; } = "";

    public double ResponseTimeMs { get; set; }

    public double CpuUsage { get; set; }

    public double MemoryUsageMB { get; set; }

    public double ErrorRate { get; set; }

    public DateTime LastChecked { get; set; }
}