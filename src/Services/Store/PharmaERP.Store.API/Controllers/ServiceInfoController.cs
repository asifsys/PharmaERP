using Microsoft.AspNetCore.Mvc;

namespace PharmaERP.Store.API.Controllers;

/// <summary>
/// Returns metadata for the Store microservice.
/// Used by the Executive Admin monitoring dashboard.
/// </summary>
[ApiController]
[Route("api/store/service")]
public class ServiceInfoController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ServiceInfoController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("info")]
    public IActionResult GetInfo()
    {
        return Ok(new
        {
            ServiceName = _configuration["Service:Name"],
            Department = _configuration["Service:Department"],
            Status = "Running",
            Version = "1.0.0",
            Timestamp = DateTime.UtcNow
        });
    }
}
