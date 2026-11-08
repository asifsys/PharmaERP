namespace PharmaERP.Dashboard.API.Configuration;

public sealed class ServiceRegistryOptions
{
    public List<ServiceEndpoint> Services { get; set; } = [];
}

public sealed class ServiceEndpoint
{
    public string Name { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string HealthUrl { get; set; } = string.Empty;
}