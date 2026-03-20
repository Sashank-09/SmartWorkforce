using Microsoft.EntityFrameworkCore;
using SmartWorkforce.Application.DTOs.Analytics;
using SmartWorkforce.Application.Interfaces;
using SmartWorkforce.Domain.Enums;
using SmartWorkforce.Infrastructure.Data;

namespace SmartWorkforce.Infrastructure.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly AppDbContext _context;

    public AnalyticsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardAnalyticsDto> GetDashboardAnalyticsAsync()
    {
        var employees = await _context.Employees
            .Where(e => !e.IsDeleted).CountAsync();

        var departments = await _context.Departments
            .Where(d => !d.IsDeleted).CountAsync();

        var tasks = await _context.Tasks
            .Where(t => !t.IsDeleted).ToListAsync();

        var departmentStats = await _context.Departments
            .Where(d => !d.IsDeleted)
            .Include(d => d.Employees)
            .ThenInclude(e => e.AssignedTasks)
            .Select(d => new DepartmentStatsDto
            {
                DepartmentName = d.Name,
                EmployeeCount = d.Employees.Count(e => !e.IsDeleted),
                TaskCount = d.Employees
                    .SelectMany(e => e.AssignedTasks)
                    .Count(t => !t.IsDeleted),
                CompletedTaskCount = d.Employees
                    .SelectMany(e => e.AssignedTasks)
                    .Count(t => !t.IsDeleted &&
                        t.Status == TaskItemStatus.Completed)
            })
            .ToListAsync();

        var last7Days = Enumerable.Range(0, 7)
            .Select(i => DateTime.UtcNow.Date.AddDays(-i))
            .OrderBy(d => d)
            .ToList();

        var taskTrends = last7Days.Select(date => new TaskTrendDto
        {
            Date = date.ToString("MMM dd"),
            TasksCreated = tasks.Count(t =>
                t.CreatedAt.Date == date),
            TasksCompleted = tasks.Count(t =>
                t.Status == TaskItemStatus.Completed &&
                t.UpdatedAt.HasValue &&
                t.UpdatedAt.Value.Date == date)
        }).ToList();

        return new DashboardAnalyticsDto
        {
            TotalEmployees = employees,
            TotalDepartments = departments,
            TotalTasks = tasks.Count,
            PendingTasks = tasks.Count(t =>
                t.Status == TaskItemStatus.Pending),
            InProgressTasks = tasks.Count(t =>
                t.Status == TaskItemStatus.InProgress),
            CompletedTasks = tasks.Count(t =>
                t.Status == TaskItemStatus.Completed),
            CancelledTasks = tasks.Count(t =>
                t.Status == TaskItemStatus.Cancelled),
            DepartmentStats = departmentStats,
            TaskTrends = taskTrends
        };
    }

    public async Task<List<EmployeePerformanceDto>> GetEmployeePerformanceAsync()
    {
        var employees = await _context.Employees
            .Where(e => !e.IsDeleted)
            .Include(e => e.Department)
            .Include(e => e.AssignedTasks)
            .ToListAsync();

        return employees.Select(e =>
        {
            var activeTasks = e.AssignedTasks
                .Where(t => !t.IsDeleted).ToList();
            var completed = activeTasks
                .Count(t => t.Status == TaskItemStatus.Completed);
            var total = activeTasks.Count;

            return new EmployeePerformanceDto
            {
                EmployeeName = $"{e.FirstName} {e.LastName}",
                DepartmentName = e.Department?.Name ?? string.Empty,
                TotalTasks = total,
                CompletedTasks = completed,
                PendingTasks = activeTasks
                    .Count(t => t.Status == TaskItemStatus.Pending),
                CompletionRate = total > 0
                    ? Math.Round((double)completed / total * 100, 2)
                    : 0
            };
        }).ToList();
    }
}