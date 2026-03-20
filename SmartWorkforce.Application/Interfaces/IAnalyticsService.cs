using SmartWorkforce.Application.DTOs.Analytics;

namespace SmartWorkforce.Application.Interfaces;

public interface IAnalyticsService
{
    Task<DashboardAnalyticsDto> GetDashboardAnalyticsAsync();
    Task<List<EmployeePerformanceDto>> GetEmployeePerformanceAsync();
}