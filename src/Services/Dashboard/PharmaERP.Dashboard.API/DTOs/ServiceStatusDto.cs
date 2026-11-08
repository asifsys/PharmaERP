namespace PharmaERP.Dashboard.API.DTOs;

public sealed class ServiceStatusDto
{
    public string ServiceName { get; set; } = "";

    public string Department { get; set; } = "";

    public string Status { get; set; } = "Unknown";

    public double ResponseTimeMs { get; set; }

    public double CpuUsage { get; set; }

    public double MemoryUsageMB { get; set; }

    public double ErrorRate { get; set; }

    public DateTime LastChecked { get; set; }
}