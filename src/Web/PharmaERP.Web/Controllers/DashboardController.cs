using Microsoft.AspNetCore.Mvc;
using PharmaERP.Web.Models;
using PharmaERP.Web.Services;

namespace PharmaERP.Web.Controllers;

public class DashboardController : Controller
{
    private readonly DashboardApiService _dashboardApi;

    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        DashboardApiService dashboardApi,
        ILogger<DashboardController> logger)
    {
        _dashboardApi = dashboardApi;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var summary =
                await _dashboardApi.GetAsync<DashboardSummaryViewModel>(
                    "dashboard/summary",
                    cancellationToken);

            return View(summary ?? new DashboardSummaryViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unable to load Executive Dashboard.");

            ViewBag.ErrorMessage =
                "Dashboard service is temporarily unavailable.";

            return View(new DashboardSummaryViewModel());
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetServices(
        CancellationToken cancellationToken)
    {
        try
        {
            var services =
                await _dashboardApi.GetAsync<
                    List<ServiceStatusViewModel>>(
                    "dashboard/services",
                    cancellationToken);

            return Json(services ?? []);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unable to retrieve service statuses.");

            return StatusCode(503, new
            {
                message = "Service monitoring is temporarily unavailable."
            });
        }
    }
}