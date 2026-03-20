using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWorkforce.Application.Interfaces;

namespace SmartWorkforce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Manager")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var analytics = await _analyticsService
            .GetDashboardAnalyticsAsync();
        return Ok(analytics);
    }

    [HttpGet("employee-performance")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetEmployeePerformance()
    {
        var performance = await _analyticsService
            .GetEmployeePerformanceAsync();
        return Ok(performance);
    }
}