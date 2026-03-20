namespace SmartWorkforce.Application.DTOs.Analytics;

public class DashboardAnalyticsDto
{
    public int TotalEmployees { get; set; }
    public int TotalDepartments { get; set; }
    public int TotalTasks { get; set; }
    public int PendingTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int CancelledTasks { get; set; }
    public List<DepartmentStatsDto> DepartmentStats { get; set; } = new();
    public List<TaskTrendDto> TaskTrends { get; set; } = new();
}

public class DepartmentStatsDto
{
    public string DepartmentName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public int TaskCount { get; set; }
    public int CompletedTaskCount { get; set; }
}

public class TaskTrendDto
{
    public string Date { get; set; } = string.Empty;
    public int TasksCreated { get; set; }
    public int TasksCompleted { get; set; }
}

public class EmployeePerformanceDto
{
    public string EmployeeName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public double CompletionRate { get; set; }
}