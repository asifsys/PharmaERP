using Microsoft.AspNetCore.Mvc;
using PharmaERP.Dashboard.API.DTOs;
using PharmaERP.Dashboard.API.Interfaces;

namespace PharmaERP.Dashboard.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    // GET: api/dashboard/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetSummaryAsync(
            cancellationToken);

        return Ok(result);
    }

    // GET: api/dashboard/services
    [HttpGet("services")]
    public async Task<IActionResult> GetServices(
        CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetServicesAsync(
            cancellationToken);

        return Ok(result);
    }

    // GET: api/dashboard/search
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] ServiceQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await _dashboardService.SearchServicesAsync(
            query,
            cancellationToken);

        return Ok(result);
    }
}