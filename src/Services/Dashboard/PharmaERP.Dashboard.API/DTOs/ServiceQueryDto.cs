namespace PharmaERP.Dashboard.API.DTOs;

public sealed class ServiceQueryDto
{
    public string? Search { get; set; }

    public string? Department { get; set; }

    public string? Status { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string SortColumn { get; set; } = "ServiceName";

    public string SortDirection { get; set; } = "asc";
}