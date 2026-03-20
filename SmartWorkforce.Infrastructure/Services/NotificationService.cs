using Microsoft.AspNetCore.SignalR;
using SmartWorkforce.Application.Interfaces;

namespace SmartWorkforce.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<DynamicHub> _hubContext;

    public NotificationService(IHubContext<DynamicHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendTaskAssignedNotificationAsync(
        string employeeUserId,
        string taskTitle,
        string assignedByName)
    {
        await _hubContext.Clients.Group(employeeUserId).SendAsync(
            "TaskAssigned",
            new
            {
                message = $"New task assigned: {taskTitle}",
                assignedBy = assignedByName,
                timestamp = DateTime.UtcNow
            });
    }

    public async Task SendTaskStatusUpdatedNotificationAsync(
        string managerUserId,
        string taskTitle,
        string newStatus,
        string employeeName)
    {
        await _hubContext.Clients.Group(managerUserId).SendAsync(
            "TaskStatusUpdated",
            new
            {
                message = $"{employeeName} updated '{taskTitle}' to {newStatus}",
                timestamp = DateTime.UtcNow
            });
    }
}

public class DynamicHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value;
        if (!string.IsNullOrEmpty(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        await base.OnConnectedAsync();
    }
}